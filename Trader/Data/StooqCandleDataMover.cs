using System.IO.Compression;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CsvHelper.TypeConversion;
using Trader.Dividends;

namespace Trader.Data;

public class StooqCandleDataMover
{
    public static async Task Move()
    {
        var symbols = DividendsStrategy.Symbols;
        await using var db = new TradingContext();

        var path = @"D:\\d_us_txt.zip";
        await using var file = File.OpenRead(path);
        using var zip = new ZipArchive(file, ZipArchiveMode.Read);
        foreach (var entry in zip.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name)) continue;
            var parts = entry.Name.Split('.');
            var symbol = parts[0];
            if (!symbols.Contains(symbol, new CaseInsensitiveValueComparer())) continue;

            await using var stream = entry.Open();
            var candles = GetCandles(stream);
            foreach (var candle in candles)
            {
                candle.Symbol = symbol.ToUpper();
            }

            await db.Candles.AddRangeAsync(candles);
            await db.SaveChangesAsync();
            Console.WriteLine($"symbol {symbol} added");
        }

        Console.WriteLine("Stooq data imported");
    }

    private static List<Candle> GetCandles(Stream stream)
    {
        var sr = new StreamReader(stream);
        using var csv = new CsvReader(sr, new CsvConfiguration(CultureInfo.InvariantCulture));
        var options = new TypeConverterOptions { Formats = new[] { "yyyyMMdd" } };
        csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
        csv.Context.RegisterClassMap<StooqCandleMap>();
        var records = csv.GetRecords<Candle>().ToList();
        return records;
    }
}