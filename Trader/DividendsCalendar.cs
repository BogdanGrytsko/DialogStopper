using HtmlAgilityPack;

namespace Trader;

public class DividendsCalendar
{
    private static string TradingViewUrl = "https://www.tradingview.com/chart/MTdLRWLd/?symbol={0}";
    private static string SeekingAlphaUrl = "https://seekingalpha.com/symbol/{0}/dividends/history";

    private readonly List<string> _symbols;
    private readonly HttpClient _httpClient;

    public DividendsCalendar()
    {
        _symbols = new List<string> { "XOM", "KO", "T", "VZ", "CVX", "MCD", "JNJ", "PFE", "IBM", "ABBV", "TGT" };
        _httpClient = new HttpClient();
    }

    public async Task Scrape()
    {
        var url = string.Format(SeekingAlphaUrl, "KO");
        var html = await _httpClient.GetStringAsync(url);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
    }
}