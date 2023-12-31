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
            Verbose = true
        };
        _readModel.Load(input);

        for (int i = 0; i < 1; i++)
        {
            input.DaysAfterExDate = i;
            SimpleTimeStrategy(input);
        }
        if (input.Verbose)
            await GoogleSheetDividends.StoreData(_trades);
    }

    private void SimpleTimeStrategy(DividendsInputParams input)
    {
        //strategy : buy at open 1(x) day before ExDate, Sale at open 0(y) days after ExDate
        var capital = input.StartCapital;
        var time = input.StartDate;
        foreach (var dividend in _readModel.Dividends)
        {
            //can't go backwards in time
            if (time >= dividend.Key.Time)
            {
                if (input.Verbose)
                    Console.WriteLine($"Skip trading {dividend.Key.Symbol}");
                continue;
            }

            time = dividend.Key.Time;

            //has to account for Monday --> Sunday! Need to buy on Friday
            var buyDate = _readModel.GetDateBefore(dividend.Key, input.DaysBeforeExDate);
            var buyPrice = _readModel.HistoricalData[dividend.Key with { Time = buyDate }].Open;
            capital += CollectDividends(buyDate);
            var capitalBeforeBuy = capital;
            var stockAmt = capital / buyPrice;

            var sellDate = _readModel.GetDateAfter(dividend.Key, input.DaysAfterExDate);
            var sellPrice = _readModel.HistoricalData[dividend.Key with { Time = sellDate }].Open;
            var dividendGain = stockAmt * dividend.Value.Amount;
            _dividendsList.Add((dividend.Value.PaymentDate, dividendGain));
            capital = stockAmt * sellPrice;

            //when does stock recover? in days
            _trades.Add(new PortfolioDividendTrade
            {
                Date = dividend.Key.Time, BuySellGain = capital - capitalBeforeBuy, EndCapital = capital,
                DividendPercent = dividend.Value.Percent, DividendGain = dividendGain, Symbol = dividend.Key.Symbol,
                //by given symbol, price and time, find next time when price >= given
                //in efficient implementation it should be just a part of time machine
                RecoversInDays = -1
            });
            if (input.Verbose)
                Console.WriteLine($"Date: {dividend.Key.Time:d}, Capital: {capital:F0}, Symbol: {dividend.Key.Symbol}");
        }

        capital += CollectDividends(input.CutOffDate);
        CalcCompounding(input, capital);

        AddProjectedFutureDates(input);
        _trades.Add(new PortfolioDividendTrade { Date = input.CutOffDate, EndCapital = capital });
    }

    private static void CalcCompounding(DividendsInputParams input, decimal capital)
    {
        var years = input.EndDate.Year - input.StartDate.Year;
        var compoundingPerYear = Math.Pow((double)capital / (double)input.StartCapital, 1 / (double)years) - 1;
        Console.WriteLine(
            $"Initial: {input.StartCapital}, End: {capital:F0}, Years: {years}, Compound per year: {compoundingPerYear * 100:F2}");
    }

    private void AddProjectedFutureDates(DividendsInputParams input)
    {
        //predict based on previous year - same quarter
        foreach (var dividend in _readModel.Dividends.Values)
        {
            var projectedDate = dividend.ExDate.AddYears(1);
            if (projectedDate <= input.CutOffDate && projectedDate > input.EndDate)
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