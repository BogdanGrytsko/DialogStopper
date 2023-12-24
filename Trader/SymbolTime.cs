namespace Trader;

public record SymbolTime(string Symbol, DateTime Time)
{
    public override string ToString()
    {
        return $"{Symbol}:{Time:d}";
    }
};