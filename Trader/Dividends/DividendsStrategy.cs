namespace Trader.Dividends;

public class DividendsStrategy
{
    //public static List<string> Symbols = new() { "XOM", "CVX", "KO", "MCD", "T", "VZ", "JNJ", "PFE", "IBM", "ABBV", "TGT" };
    public static List<string> Symbols = new() { "XOM" };

    private Dictionary<SymbolTime, Candle> _historicalData;
    private SortedDictionary<SymbolTime, Dividend> _dividends;
    private readonly TradingContext _context;

    public DividendsStrategy()
    {
        _context = new TradingContext();
    }

    public async Task Run()
    {
        var startDate = new DateTime(2021, 10, 1);
        _historicalData = GetHistoricalData(startDate);
        _dividends = GetDividendsData(startDate);
        MapHistoricalData();

        //strategy : buy at open 1(x) day before ExDate, Sale at open 0(y) days after ExDate
        var startCapital = 100000m;
        var capital = startCapital;
        var daysBeforeExDate = 1;
        var daysAfterExDate = 0;
        foreach (var dividend in _dividends.Where(x => x.Value.Candle != null))
        {
            //has to account for Monday --> Sunday! Need to buy on Friday
            var buyDate = GetDateBefore(dividend.Key, daysBeforeExDate);
            var buyPrice = _historicalData[dividend.Key with { Time = buyDate }].Open;
            var stockAmt = capital / buyPrice;
            var sellDate = GetDateAfter(dividend.Key, daysAfterExDate);
            var sellPrice = _historicalData[dividend.Key with { Time = sellDate }].Open;
            var dividendGain = stockAmt * dividend.Value.Amount;
            capital = stockAmt * sellPrice;
            capital += dividendGain;
            Console.WriteLine($"Date: {dividend.Key.Time:d}, Capital: {capital}");
        }

        Console.WriteLine($"Initial capital: {startCapital}, End Capital: {capital:F0}");
    }

    private SortedDictionary<SymbolTime, Dividend> GetDividendsData(DateTime startDate)
    {
        var dividends = _context.Dividends.Where(x => Symbols.Contains(x.Symbol) && x.ExDate >= startDate);
        var sortedDictionary = new SortedDictionary<SymbolTime, Dividend>();
        foreach (var kvp in dividends)
        {
            var key = new SymbolTime(kvp.Symbol, kvp.ExDate);
            sortedDictionary.Add(key, kvp);
        }
        return sortedDictionary;
    }

    private Dictionary<SymbolTime, Candle> GetHistoricalData(DateTime startDate)
    {
        var candles = _context.Candles.Where(x => Symbols.Contains(x.Symbol) && x.Date >= startDate);
        var dictionary = new Dictionary<SymbolTime, Candle>();
        foreach (var candle in candles)
        {
            dictionary.TryAdd(new SymbolTime(candle.Symbol, candle.Date), candle);
        }
        return dictionary;
    }

    private DateTime GetDateBefore(SymbolTime symbolTime, int daysBeforeExDate)
    {
        if (daysBeforeExDate > 10)
            throw new Exception("daysBeforeExDate > 10");
        var date = symbolTime.Time.AddDays(-daysBeforeExDate);
        if (!_historicalData.ContainsKey(symbolTime with { Time = date }))
            return GetDateBefore(symbolTime, daysBeforeExDate + 1);
        return date;
    }

    private DateTime GetDateAfter(SymbolTime symbolTime, int daysAfterExDate)
    {
        if (daysAfterExDate > 10)
            throw new Exception("daysAfterExDate > 10");
        var date = symbolTime.Time.AddDays(daysAfterExDate);
        if (!_historicalData.ContainsKey(symbolTime with { Time = date }))
            return GetDateBefore(symbolTime, daysAfterExDate + 1);
        return date;
    }

    private void MapHistoricalData()
    {
        foreach (var dividend in _dividends)
        {
            if (_historicalData.TryGetValue(dividend.Key, out var candle))
            {
                dividend.Value.Candle = candle;
                dividend.Value.Percent = dividend.Value.Amount / candle.Open;
            }
        }
    }
}