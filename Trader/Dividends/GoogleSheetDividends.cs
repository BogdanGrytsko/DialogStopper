using DialogStopper.Storage;
using System.Globalization;

namespace Trader.Dividends;

public class GoogleSheetDividends
{
    private const string SheetId = "1Orepmp0hJiVjRQ_qVR54oZbwukHvPg9pQlUL33q3QYo";

    public static async Task<SortedDictionary<SymbolTime, Dividend>> LoadDividendsData(string symbol)
    {
        var storage = new GoogleSheetStorage<DividendDto>(SheetId) { SheetName = symbol };
        var data = await storage.GetAsync();
        var mapped = Map(data);
        var sortedDictionary = new SortedDictionary<SymbolTime, Dividend>();
        foreach (var kvp in mapped)
        {
            var key = new SymbolTime(symbol, kvp.ExDate);
            //happened very rarely
            if (!sortedDictionary.ContainsKey(key))
                sortedDictionary.Add(key, kvp);
            else
            {
                Console.WriteLine($"{key} had duplicates!");
            }
        }
        return sortedDictionary;
    }

    public static async Task StoreData(List<PortfolioDividendTrade> trades)
    {
        var storage = new GoogleSheetStorage<PortfolioDividendTrade>(SheetId) { SheetName = "Calendar" };
        await storage.DeleteAsync(1, 500);
        await storage.AddAsync(trades, true);
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
}