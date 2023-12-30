namespace Trader.Dividends;

public class DividendsDataMover
{
    public static async Task Move()
    {
        await using var db = new TradingContext();
        foreach (var symbol in DividendsStrategy.Symbols)
        {
            var data = await GoogleSheetDividends.LoadDividendsData(symbol);
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