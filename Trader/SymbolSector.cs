using System.ComponentModel.DataAnnotations;

namespace Trader;

public class SymbolSector
{
    public SymbolSector(string symbol, Sector sector)
    {
        Symbol = symbol;
        Sector = sector;
    }

    public int Id { get; set; }
    [Required, StringLength(4)]
    public string Symbol { get; set; }
    [Required]
    public Sector Sector { get; set; }
}