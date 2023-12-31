namespace Trader.Dividends;

public class DividendsInputParams
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal StartCapital { get; set; } = 100000;
    public DateTime CutOffDate => EndDate.AddMonths(3);
}