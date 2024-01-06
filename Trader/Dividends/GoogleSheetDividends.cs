using DialogStopper.Storage;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
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

    public static async Task AddPivotData(SortedDictionary<SymbolTime, decimal> profit)
    {
        var storage = new GoogleSheetStorage<PortfolioDividendTrade>(SheetId) { SheetName = "PerYear" };
        await storage.DeleteAsync(10, 1, 500);

        var valueRange = new ValueRange { Values = new List<IList<object>>() };
        var headers = new List<object> { "Symbol/Year" };
        var times = profit.Keys.Select(x => x.Time).Distinct().ToList();
        foreach (var header in times.Select(x => x.Year))
        {
            headers.Add(header);
        }
        valueRange.Values.Add(headers);

        foreach (var symbol in profit.Keys.Select(x => x.Symbol).Distinct().OrderBy(x => x))
        {
            var rowValues = new List<object> { symbol };
            foreach (var time in times)
            {
                var key = new SymbolTime(symbol, time);
                profit.TryGetValue(key, out var value);
                GoogleSheetStorageHelper.AddValue(value, rowValues);
            }
            valueRange.Values.Add(rowValues);
        }

        var appendRequest = storage.SheetsService.Spreadsheets.Values.Append(valueRange, SheetId, storage.GetRange(times.Count));
        appendRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        var _ = await appendRequest.ExecuteAsync();
    }
}