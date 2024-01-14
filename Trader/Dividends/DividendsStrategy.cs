namespace Trader.Dividends;

public class DividendsStrategy
{
    //either VZ or T
    //either PEP or BAC
    //COP intersects with XOM. 
    public static List<string> Symbols = new()
    {
        "XOM", "CVX", "COP", "KO", "MCD", "T", "IBM",
        "CL", "PEP", "PM", "SBUX", "EOG", "PXD", "FANG", "PSX", "VLO"
    };
    
    private readonly List<(DateTime, decimal)> _dividendsList;
    private readonly List<PortfolioDividendTrade> _trades;
    private readonly DividendsReadModel _readModel;
    private readonly SortedDictionary<SymbolTime, decimal> _profitDictionary;
    public const int SumYear = 3000;

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

        var endCapital = SimpleTimeStrategy(input);
        CalcCompounding(input, endCapital);
        AddProjectedFutureDates(input);
        await GoogleSheetDividends.StoreData(_trades);
    }

    public async Task RunPerSymbolPerYear()
    {
        var input = new DividendsInputParams
        {
            StartDate = new DateTime(2020, 1, 1),
            EndDate = new DateTime(2024, 1, 1),
            Verbose = false
        };
        _readModel.Load(input);

        for (var i = input.StartDate; i < input.EndDate; i = i.AddYears(1))
        {
            foreach (var symbol in _readModel.SymbolSector)
            {
                SimpleTimeStrategy(input with { StartDate = i, EndDate = i.AddYears(1), Symbols = new List<string> { symbol.Key }});
                GetProfitPerSymbolPerYear();
                _trades.Clear();
            }
        }

        ApplyAverage();
        await GoogleSheetDividends.AddPivotData(_profitDictionary);
    }

    private void ApplyAverage()
    {
        var totalYears = _profitDictionary.Select(x => x.Key.Time.Year).Distinct().Count() - 1;
        foreach (var pair in _profitDictionary.ToList()
                     .Where(x => x.Key.Time == new DateTime(SumYear, 1, 1)))
        {
            _profitDictionary[pair.Key] /= totalYears;
        }
    }

    private void GetProfitPerSymbolPerYear()
    {
        foreach (var trade in _trades)
        {
            var key = new SymbolTime(trade.Symbol, new DateTime(trade.Date.Year, 1, 1));
            AddValue(key, trade.Profit);

            var sumKey = new SymbolTime(trade.Symbol, new DateTime(SumYear, 1, 1));
            AddValue(sumKey, trade.Profit);
        }
    }

    private void AddValue(SymbolTime key, decimal profit)
    {
        if (!_profitDictionary.ContainsKey(key))
            _profitDictionary.Add(key, 0);
        _profitDictionary[key] += profit;
    }

    private decimal SimpleTimeStrategy(DividendsInputParams input)
    {
        //strategy : buy at open 1(x) day before ExDate, Sale at open 0(y) days after ExDate
        var capital = input.StartCapital;
        var time = input.StartDate;
        foreach (var dividend in _readModel.Dividends
                     .Where(x => input.Symbols.Contains(x.Key.Symbol) 
                                 && x.Key.Time >= input.StartDate && x.Key.Time < input.EndDate))
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
        _trades.Add(new PortfolioDividendTrade
        {
            Date = input.EndDate.AddDays(-1), Symbol = "A1l", EndCapital = capital,
            BuySellGain = _trades.Sum(x => x.BuySellGain), DividendGain = _trades.Sum(x => x.DividendGain)
        });
        return capital;
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