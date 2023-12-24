using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Trader.Data;

namespace Trader;

public class DividendsStrategy
{
    public void Run()
    {
        var historicalData = LoadHistoricalData();
    }

    private static Dictionary<SymbolTime, Candle> LoadHistoricalData()
    {
        var directoryPath = "Data";
        var dictionary = new Dictionary<SymbolTime, Candle>();
        var fileNames = Directory.GetFiles(directoryPath);
        foreach (var filePath in fileNames)
        {
            var parts = Path.GetFileName(filePath).Split('_');
            var symbol = parts[0];
            var candles = GetCandles(filePath);
            foreach (var candle in candles)
            {
                dictionary.TryAdd(new SymbolTime(symbol, candle.Date), candle);
            }
        }
        return dictionary;
    }

    private static List<Candle> GetCandles(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
        csv.Context.RegisterClassMap<MarketWatchCandleMap>();
        var records = csv.GetRecords<Candle>().ToList();
        return records;
    }
}