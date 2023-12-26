namespace Trader.Dividends;

public class DividendsDataMover
{
    public static async Task Move()
    {
        var symbols = new List<string> { "XOM", "CVX", "KO", "MCD", "T", "VZ", "JNJ", "PFE", "IBM", "ABBV", "TGT" };
        await using var db = new TradingContext();
        foreach (var symbol in symbols)
        {
            var data = await GoogleSheetDividendsLoader.LoadDividendsData(symbol);
            foreach (var dividend in data.Values)
            {
                dividend.Symbol = symbol;
            }
            await db.Dividends.AddRangeAsync(data.Values);
            await db.SaveChangesAsync();
            Console.WriteLine($"symbol {symbol} added");
        }
    }
}