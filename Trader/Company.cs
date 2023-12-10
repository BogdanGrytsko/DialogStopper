namespace Trader;

public class Company
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public List<Dividend> Dividends { get; set; }
}