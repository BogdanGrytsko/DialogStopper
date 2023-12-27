﻿using Trader.Data;

namespace Trader.Dividends;

public class DividendsStrategy
{
    private Dictionary<SymbolTime, Candle> _historicalData;
    private SortedDictionary<SymbolTime, Dividend> _dividends;

    public async Task Run()
    {
        _historicalData = MarketWatchDataLoader.LoadHistoricalData();
        _dividends = await GoogleSheetDividendsLoader.LoadDividendsData("XOM");
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