﻿namespace Trader.Dividends;

public class DividendsReadModel
{
    public SortedDictionary<SymbolTime, Candle> HistoricalData { get; private set; }
    public SortedDictionary<SymbolTime, Dividend> Dividends { get; private set; }
    public Dictionary<string, string> SymbolSector { get; private set; }
    private readonly TradingContext _context;

    public DividendsReadModel(TradingContext context)
    {
        _context = context;
    }

    public void Load(DividendsInputParams input)
    {
        HistoricalData = GetHistoricalData(input);
        Dividends = GetDividendsData(input);
        MapHistoricalData();
        SymbolSector = GetSymbolSectorData(input);
    }

    private Dictionary<string, string> GetSymbolSectorData(DividendsInputParams input)
    {
        return _context.SymbolSectors
            .Where(x => input.Symbols.Contains(x.Symbol))
            .ToDictionary(x => x.Symbol, x => x.SectorName);
    }

    private SortedDictionary<SymbolTime, Dividend> GetDividendsData(DividendsInputParams input)
    {
        var dividends = _context.Dividends.Where(x => input.Symbols.Contains(x.Symbol) && x.ExDate >= input.StartDate && x.ExDate <= input.EndDate);
        var sortedDictionary = new SortedDictionary<SymbolTime, Dividend>();
        foreach (var kvp in dividends)
        {
            var key = new SymbolTime(kvp.Symbol, kvp.ExDate);
            sortedDictionary.Add(key, kvp);
        }
        return sortedDictionary;
    }

    private SortedDictionary<SymbolTime, Candle> GetHistoricalData(DividendsInputParams input)
    {
        var candles = _context.Candles.Where(x => input.Symbols.Contains(x.Symbol) && x.Date >= input.StartDate && x.Date <= input.EndDate);
        var dictionary = new SortedDictionary<SymbolTime, Candle>();
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
        //has to account for Monday --> Sunday! Need to buy on Friday
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

    public int RecoveryAfterExDateInDays(KeyValuePair<SymbolTime, Dividend> dividend, decimal buyPrice, DateTime buyDate)
    {
        //in efficient implementation it should be just a part of time machine
        return ((HistoricalData.Values
            .FirstOrDefault(x => x.Symbol == dividend.Key.Symbol && x.Date > buyDate && x.High >= buyPrice)?
            .Date ?? DateTime.MaxValue) - dividend.Key.Time).Days;
    }
}