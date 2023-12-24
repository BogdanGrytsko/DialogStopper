namespace Trader.Dividends;

public class Dividend
{
    public DateTime ExDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Percent { get; set; }
    public DateTime PaymentDate { get; set; }
}