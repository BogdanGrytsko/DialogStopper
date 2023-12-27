using System.ComponentModel.DataAnnotations;

namespace Trader.Dividends;

public class Dividend
{
    public int Id { get; set; }
    [Required, StringLength(4)]
    public string Symbol { get; set; }
    public DateTime ExDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Percent { get; set; }
    public DateTime PaymentDate { get; set; }

    public virtual Candle Candle { get; set; }
}