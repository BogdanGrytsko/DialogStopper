namespace Trader.Dividends;

public class DividendsStrategy
{
    //F, SBUX, HD are BAD in the last 3 years. PG also
    //either VZ or T
    //COP intersects with XOM. Only last year JPM was bad
    //ABBV, PFE, JNJ - healthcare, bad
    public static List<string> Symbols = new()
    {
        "XOM", "CVX", "KO", "MCD", "T", "VZ", "IBM", "TGT",
        "JPM", "BAC", "CL", "PEP", "PM"
    };
    
    private readonly List<(DateTime, decimal)> _dividendsList;
    private readonly List<PortfolioDividendTrade> _trades;
    private readonly DividendsReadModel _readModel;
    private readonly SortedDictionary<SymbolTime, decimal> _profitDictionary;

    public DividendsStrategy(DividendsReadModel dividendsReadModel)
    {
        _dividendsList = new List<(DateTime, decimal)>();
        _trades = new List<PortfolioDividendTrade>();
        _readModel = dividendsReadModel;
        _profitDictionary = new SortedDictionary<SymbolTime, decimal>();
    }

    public async Task RunCalendar()
    {
        var input = new DividendsInputParams
        {
            StartDate = new DateTime(2023, 1, 1),
            EndDate = new DateTime(2024, 1, 1),
            Symbols = Symbols,
            Verbose = true
        };
        _readModel.Load(input);

        SimpleTimeStrategy(input);
        if (input.Verbose)
            await GoogleSheetDividends.StoreData(_trades);
    }

    public async Task RunPerYearPivotResults()
    {
        var input = new DividendsInputParams
        {
            StartDate = new DateTime(2023, 1, 1),
            EndDate = new DateTime(2024, 1, 1),
            Symbols = Symbols,
            Verbose = false
        };
        _readModel.Load(input);

        for (var i = input.StartDate; i < input.EndDate; i = i.AddYears(1))
        {
            SimpleTimeStrategy(input with { StartDate = i, EndDate = i.AddYears(1)});
            //todo can be extended to include all time sums per symbol
            GetProfitPerSymbolPerYear();
            _trades.Clear();
        }
        await GoogleSheetDividends.AddPivotData(_profitDictionary);
    }

    private void GetProfitPerSymbolPerYear()
    {
        foreach (var trade in _trades)
        {
            var key = new SymbolTime(trade.Symbol, new DateTime(trade.Date.Year, 1, 1));
            if (!_profitDictionary.ContainsKey(key))
                _profitDictionary.Add(key, 0);
            _profitDictionary[key] += trade.Profit;
        }
    }

    private void SimpleTimeStrategy(DividendsInputParams input)
    {
        //strategy : buy at open 1(x) day before ExDate, Sale at open 0(y) days after ExDate
        var capital = input.StartCapital;
        var time = input.StartDate;
        foreach (var dividend in _readModel.Dividends)
        {
            var sector = _readModel.SymbolSector[dividend.Key.Symbol];
            //can't go backwards in time
            if (time >= dividend.Key.Time)
            {
                if (input.Verbose)
                    Console.WriteLine($"Skip trading {dividend.Key.Symbol}");
                continue;
            }

            time = dividend.Key.Time;

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

            _trades.Add(new PortfolioDividendTrade
            {
                Date = dividend.Key.Time, BuySellGain = capital - capitalBeforeBuy, EndCapital = capital,
                DividendPercent = dividend.Value.Percent, DividendGain = dividendGain, Symbol = dividend.Key.Symbol,
                RecoversInDays = _readModel.RecoveryAfterExDateInDays(dividend, buyPrice, buyDate),
                Sector = sector
            });
            if (input.Verbose)
                Console.WriteLine($"Date: {dividend.Key.Time:d}, Capital: {capital:F0}, Symbol: {dividend.Key.Symbol}");
        }

        capital += CollectDividends(input.CutOffDate);
        CalcCompounding(input, capital);
        CalcProfitPerSymbol();

        //this is relevant only for calendar!
        AddProjectedFutureDates(input);
        _trades.Add(new PortfolioDividendTrade
        {
            Date = input.EndDate.AddDays(-1), Symbol = "All", EndCapital = capital,
            BuySellGain = _trades.Sum(x => x.BuySellGain), DividendGain = _trades.Sum(x => x.DividendGain)
        });
    }

    private void CalcProfitPerSymbol()
    {
        foreach (var group in _trades.GroupBy(x => x.Symbol)
                     .OrderByDescending(x => x.Sum(y => y.Profit)))
        {
            var profitability = group.Sum(x => x.Profit);
            Console.WriteLine($"{group.Key}: {profitability:F0}; {group.First().Sector}");
        }
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
                _trades.Add(new PortfolioDividendTrade
                {
                    Date = projectedDate, Symbol = dividend.Symbol, DividendPercent = dividend.Percent, Sector = _readModel.SymbolSector[dividend.Symbol]
                });
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