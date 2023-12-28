namespace Trader.Dividends;

public class DividendsStrategy
{
    //either VZ or T
    public static List<string> Symbols = new() { "XOM", "CVX", "KO", "MCD", "T", "VZ", "JNJ", "PFE", "IBM", "ABBV", "TGT" };
    
    private Dictionary<SymbolTime, Candle> _historicalData;
    private SortedDictionary<SymbolTime, Dividend> _dividends;
    private readonly TradingContext _context;
    private readonly List<(DateTime, decimal)> _dividendsList;

    public DividendsStrategy()
    {
        _context = new TradingContext();
        _dividendsList = new List<(DateTime, decimal)>();
    }

    public async Task Run()
    {
        var startDate = new DateTime(2022, 1, 1);
        var endDate = new DateTime(2024, 1, 1);
        _historicalData = GetHistoricalData(startDate, endDate);
        _dividends = GetDividendsData(startDate, endDate);
        MapHistoricalData();

        //strategy : buy at open 1(x) day before ExDate, Sale at open 0(y) days after ExDate
        var startCapital = 100000m;
        var capital = startCapital;
        var time = startDate;
        var daysBeforeExDate = 1;
        var daysAfterExDate = 0;
        foreach (var dividend in _dividends)
        {
            //can't go backwards in time
            if (time >= dividend.Key.Time)
            {
                Console.WriteLine($"Skip trading {dividend.Key.Symbol}");
                continue;
            }
            time = dividend.Key.Time;

            //has to account for Monday --> Sunday! Need to buy on Friday
            var buyDate = GetDateBefore(dividend.Key, daysBeforeExDate);
            var buyPrice = _historicalData[dividend.Key with { Time = buyDate }].Open;

            capital += CollectDividends(buyDate);

            var stockAmt = capital / buyPrice;
            var sellDate = GetDateAfter(dividend.Key, daysAfterExDate);
            var sellPrice = _historicalData[dividend.Key with { Time = sellDate }].Open;
            var dividendGain = stockAmt * dividend.Value.Amount;
            _dividendsList.Add((dividend.Value.PaymentDate, dividendGain));
            capital = stockAmt * sellPrice;

            Console.WriteLine($"Date: {dividend.Key.Time:d}, Capital: {capital:F0}, Symbol: {dividend.Key.Symbol}");
        }

        capital += CollectDividends(endDate.AddMonths(4));

        var years =  endDate.Year - startDate.Year;
        var compoundingPerYear = Math.Pow((double)capital / (double)startCapital, 1 / (double)years) - 1;
        Console.WriteLine($"Initial: {startCapital}, End: {capital:F0}, Years: {years}, Compound per year: {compoundingPerYear * 100:F2}");
    }

    private decimal CollectDividends(DateTime buyDate)
    {
        var dividendsToCollect = _dividendsList.Where(x => x.Item1 <= buyDate).Sum(x => x.Item2);
        _dividendsList.RemoveAll(x => x.Item1 <= buyDate);
        return dividendsToCollect;
    }

    private SortedDictionary<SymbolTime, Dividend> GetDividendsData(DateTime startDate, DateTime endDate)
    {
        var dividends = _context.Dividends.Where(x => Symbols.Contains(x.Symbol) && x.ExDate >= startDate && x.ExDate <= endDate);
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
        var candles = _context.Candles.Where(x => Symbols.Contains(x.Symbol) && x.Date >= startDate && x.Date <= endDate);
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