namespace Trader.Dividends;

public class PortfolioDividendTrade
{
    public DateTime Date { get; set; }
    public string Symbol { get; set; }
    public decimal Capital { get; set; }
    public decimal DividendGain { get; set; }
    public decimal DividendPercent { get; set; }
}