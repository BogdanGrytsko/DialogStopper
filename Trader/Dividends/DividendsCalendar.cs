namespace Trader.Dividends;

public class DividendsCalendar
{
    private static string TradingViewUrl = "https://www.tradingview.com/chart/MTdLRWLd/?symbol={0}";

    private readonly List<string> _symbols;
    private readonly HttpClient _httpClient;

    public DividendsCalendar()
    {
        _symbols = new List<string> { "XOM", "KO", "T", "VZ", "CVX", "MCD", "JNJ", "PFE", "IBM", "ABBV", "TGT" };
        _httpClient = new HttpClient();
    }

    public async Task Scrape()
    {
    }

    public async Task<string> GetDataFromSA(string symbol)
    {
        var seekingAlphaUrl = "https://seekingalpha.com/symbol/{0}/dividends/history";
        var url = string.Format(seekingAlphaUrl, symbol);
        return await _httpClient.GetStringAsync(url);
    }

    public async Task<string> GetDataFromNasdaq(string symbol)
    {
        //doesn't work
        var nasdaqUrl = "https://www.nasdaq.com/market-activity/stocks/{0}/dividend-history"; ;
        var url = string.Format(nasdaqUrl, symbol);
        return await _httpClient.GetStringAsync(url);
    }

    public async Task<string> GetDataFromStreetInsider(string symbol)
    {
        var urlTemplate = "https://www.streetinsider.com/dividend_history.php?q={0}";
        var url = string.Format(urlTemplate, symbol);
        return await _httpClient.GetStringAsync(url);
    }
}