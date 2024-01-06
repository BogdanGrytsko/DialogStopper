namespace Trader.Dividends;

public record DividendsInputParams
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> Symbols { get; set; }
    public decimal StartCapital { get; set; } = 100000;
    public DateTime CutOffDate => EndDate.AddMonths(3);

    public int DaysBeforeExDate { get; set; } = 1;
    public int DaysAfterExDate { get; set; } = 0;
    public bool Verbose { get; set; } = false;
}