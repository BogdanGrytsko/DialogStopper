namespace Trader.Dividends;

public class DividendsStrategy
{
    //either VZ or T
    public static List<string> Symbols = new() { "XOM", "CVX", "KO", "MCD", "T", "VZ", "JNJ", "PFE", "IBM", "ABBV", "TGT" };
    
    private readonly List<(DateTime, decimal)> _dividendsList;
    private readonly List<PortfolioDividendTrade> _trades;
    private readonly DividendsReadModel _readModel;

    public DividendsStrategy(DividendsReadModel dividendsReadModel)
    {
        _dividendsList = new List<(DateTime, decimal)>();
        _trades = new List<PortfolioDividendTrade>();
        _readModel = dividendsReadModel;
    }

    public async Task Run()
    {
        var input = new DividendsInputParams
        {
            StartDate = new DateTime(2022, 1, 1),
            EndDate = new DateTime(2024, 1, 1),
        };
        _readModel.Load(input);

        SimpleTimeStrategy(input);
        await GoogleSheetDividends.StoreData(_trades);
    }

    private void SimpleTimeStrategy(DividendsInputParams input)
    {
        //strategy : buy at open 1(x) day before ExDate, Sale at open 0(y) days after ExDate
        var capital = input.StartCapital;
        var time = input.StartDate;
        var daysBeforeExDate = 1;
        var daysAfterExDate = 0;
        foreach (var dividend in _readModel.Dividends)
        {
            //can't go backwards in time
            if (time >= dividend.Key.Time)
            {
                Console.WriteLine($"Skip trading {dividend.Key.Symbol}");
                continue;
            }

            time = dividend.Key.Time;

            //has to account for Monday --> Sunday! Need to buy on Friday
            var buyDate = _readModel.GetDateBefore(dividend.Key, daysBeforeExDate);
            var buyPrice = _readModel.HistoricalData[dividend.Key with { Time = buyDate }].Open;
            capital += CollectDividends(buyDate);
            var capitalBeforeBuy = capital;
            var stockAmt = capital / buyPrice;

            //so impement w8 for selling
            var sellDate = _readModel.GetDateAfter(dividend.Key, daysAfterExDate);
            var sellPrice = _readModel.HistoricalData[dividend.Key with { Time = sellDate }].Open;
            var dividendGain = stockAmt * dividend.Value.Amount;
            _dividendsList.Add((dividend.Value.PaymentDate, dividendGain));
            capital = stockAmt * sellPrice;

            _trades.Add(new PortfolioDividendTrade
            {
                Date = dividend.Key.Time, BuySellGain = capital - capitalBeforeBuy, EndCapital = capital,
                DividendPercent = dividend.Value.Percent, DividendGain = dividendGain, Symbol = dividend.Key.Symbol
            });
            Console.WriteLine($"Date: {dividend.Key.Time:d}, Capital: {capital:F0}, Symbol: {dividend.Key.Symbol}");
        }

        capital += CollectDividends(input.CutOffDate);
        CalcCompounding(input, capital);

        AddProjectedFutureDates(input.EndDate);
        _trades.Add(new PortfolioDividendTrade { Date = input.CutOffDate, EndCapital = capital });
    }

    private static void CalcCompounding(DividendsInputParams input, decimal capital)
    {
        var years = input.EndDate.Year - input.StartDate.Year;
        var compoundingPerYear = Math.Pow((double)capital / (double)input.StartCapital, 1 / (double)years) - 1;
        Console.WriteLine(
            $"Initial: {input.StartCapital}, End: {capital:F0}, Years: {years}, Compound per year: {compoundingPerYear * 100:F2}");
    }

    private void AddProjectedFutureDates(DateTime endDate)
    {
        //predict based on previous year - same quarter
        foreach (var dividend in _readModel.Dividends.Values)
        {
            var projectedDate = dividend.ExDate.AddYears(1);
            if (projectedDate <= endDate.AddMonths(3) && projectedDate > endDate)
            {
                _trades.Add(new PortfolioDividendTrade { Date = projectedDate, Symbol = dividend.Symbol, DividendPercent = dividend.Percent });
            }
        }
    }

    private decimal CollectDividends(DateTime buyDate)
    {
        var dividendsToCollect = _dividendsList.Where(x => x.Item1 <= buyDate).Sum(x => x.Item2);
        _dividendsList.RemoveAll(x => x.Item1 <= buyDate);
        return dividendsToCollect;
    }
}