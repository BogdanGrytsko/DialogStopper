using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using DialogStopper.Storage;
using Trader.Data;

namespace Trader.Dividends;

public class DividendsStrategy
{
    public async Task Run()
    {
        var historicalData = LoadHistoricalData();
        var dividendsData = await LoadDividendsData();
    }

    private async Task<List<Dividend>> LoadDividendsData()
    {
        const string sheetId = "1Orepmp0hJiVjRQ_qVR54oZbwukHvPg9pQlUL33q3QYo";
        var storage = new GoogleSheetStorage<DividendDto>(sheetId)
        {
            SheetName = "XOM"
        };
        var data = await storage.Get();
        return Map(data).ToList();
    }

    private static IEnumerable<Dividend> Map(IEnumerable<DividendDto> data)
    {
        return data.Select(dividendDto => new Dividend
        {
            ExDate = dividendDto.ExDate,
            PaymentDate = dividendDto.PaymentDate,
            Amount = decimal.Parse(dividendDto.CashAmount, NumberStyles.Currency)
        });
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