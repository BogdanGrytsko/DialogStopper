namespace Trader.Dividends;

public class DividendDto
{
    public DateTime ExDate { get; set; } 
    public string Type { get; set; }
    public string CashAmount { get; set; }
    public DateTime DeclarationDate { get; set; }
    public DateTime RecordDate { get; set; }
    public DateTime PaymentDate { get; set; }
}