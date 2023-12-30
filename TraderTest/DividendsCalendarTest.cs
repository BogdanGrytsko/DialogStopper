using System.IO;
using System.Threading.Tasks;
using Trader.Dividends;
using Xunit;

namespace TraderTest;

public class DividendsCalendarTest
{
    private readonly DividendsScrapeAttempt _sut;

    public DividendsCalendarTest()
    {
        _sut = new DividendsScrapeAttempt();
    }

    [Fact]
    public async Task GetDataFromSA()
    {
        var symbol = "KO";
        var html = await _sut.GetDataFromSA(symbol);
        await File.WriteAllTextAsync($"SA_{symbol}.html", html);
    }

    [Fact]
    public async Task GetDataFromNasdaq()
    {
        var symbol = "XOM";
        var html = await _sut.GetDataFromNasdaq(symbol);
        await File.WriteAllTextAsync($"Nasdaq_{symbol}.html", html);
    }

    [Fact]
    public async Task GetDataFromStreetInsider()
    {
        var symbol = "XOM";
        var html = await _sut.GetDataFromStreetInsider(symbol);
        await File.WriteAllTextAsync($"StreetInsider_{symbol}.html", html);
    }
}