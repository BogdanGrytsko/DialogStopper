using System.ComponentModel.DataAnnotations;

namespace Trader;

public class SymbolSector
{
    public SymbolSector(string symbol, string sectorName)
    {
        Symbol = symbol;
        SectorName = sectorName;
    }

    public int Id { get; set; }
    [Required, StringLength(4)]
    public string Symbol { get; set; }
    [Required]
    public string SectorName { get; set; }
}