namespace Trader.Dividends;

public class DividendsReadModel
{
    public Dictionary<SymbolTime, Candle> HistoricalData { get; private set; }
    public SortedDictionary<SymbolTime, Dividend> Dividends { get; private set; }
    private readonly TradingContext _context;

    public DividendsReadModel(TradingContext context)
    {
        _context = context;
    }

    public void Load(DividendsInputParams input)
    {
        HistoricalData = GetHistoricalData(input.StartDate, input.EndDate);
        Dividends = GetDividendsData(input.StartDate, input.EndDate);
        MapHistoricalData();
    }

    private SortedDictionary<SymbolTime, Dividend> GetDividendsData(DateTime startDate, DateTime endDate)
    {
        var dividends = _context.Dividends.Where(x => DividendsStrategy.Symbols.Contains(x.Symbol) && x.ExDate >= startDate && x.ExDate <= endDate);
        var sortedDictionary = new SortedDictionary<SymbolTime, Dividend>();
        foreach (var kvp in dividends)
        {
            var key = new SymbolTime(kvp.Symbol, kvp.ExDate);
            sortedDictionary.Add(key, kvp);
        }
        return sortedDictionary;
    }

    private Dictionary<SymbolTime, Candle> GetHistoricalData(DateTime startDate, DateTime endDate)
    {
        var candles = _context.Candles.Where(x => DividendsStrategy.Symbols.Contains(x.Symbol) && x.Date >= startDate && x.Date <= endDate);
        var dictionary = new Dictionary<SymbolTime, Candle>();
        foreach (var candle in candles)
        {
            dictionary.TryAdd(new SymbolTime(candle.Symbol, candle.Date), candle);
        }
        return dictionary;
    }

    private void MapHistoricalData()
    {
        foreach (var dividend in Dividends)
        {
            if (HistoricalData.TryGetValue(dividend.Key, out var candle))
            {
                dividend.Value.Candle = candle;
                dividend.Value.Percent = (dividend.Value.Amount / candle.Open) * 100;
            }
        }
    }

    public DateTime GetDateBefore(SymbolTime symbolTime, int daysBeforeExDate)
    {
        if (daysBeforeExDate > 20)
            throw new Exception("daysBeforeExDate > 20");
        var date = symbolTime.Time.AddDays(-daysBeforeExDate);
        if (!HistoricalData.ContainsKey(symbolTime with { Time = date }))
            return GetDateBefore(symbolTime, daysBeforeExDate + 1);
        return date;
    }

    public DateTime GetDateAfter(SymbolTime symbolTime, int daysAfterExDate)
    {
        if (daysAfterExDate > 20)
            throw new Exception("daysAfterExDate > 20");
        var date = symbolTime.Time.AddDays(daysAfterExDate);
        if (!HistoricalData.ContainsKey(symbolTime with { Time = date }))
            return GetDateAfter(symbolTime, daysAfterExDate + 1);
        return date;
    }
}