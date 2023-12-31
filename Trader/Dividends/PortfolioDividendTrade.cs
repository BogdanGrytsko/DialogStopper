namespace Trader.Dividends;

public class PortfolioDividendTrade
{
    public DateTime Date { get; set; }
    public string Symbol { get; set; }
    public decimal BuySellGain { get; set; }
    public decimal EndCapital { get; set; }
    public decimal DividendGain { get; set; }
    public decimal DividendPercent { get; set; }
    public int RecoversInDays { get; set; }
}