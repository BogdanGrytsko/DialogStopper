using DialogStopper.Storage;
using System.Globalization;

namespace Trader.Dividends;

public class GoogleSheetDividendsLoader
{
    public static async Task<SortedDictionary<SymbolTime, Dividend>> LoadDividendsData(string symbol)
    {
        const string sheetId = "1Orepmp0hJiVjRQ_qVR54oZbwukHvPg9pQlUL33q3QYo";
        var storage = new GoogleSheetStorage<DividendDto>(sheetId)
        {
            SheetName = symbol
        };
        var data = await storage.Get();
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