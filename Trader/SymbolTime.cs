using static System.String;

namespace Trader;

public record SymbolTime(string Symbol, DateTime Time) : IComparable<SymbolTime>
{
    public int CompareTo(SymbolTime other)
    {
        if (Time != other.Time)
            return Time.CompareTo(other.Time);
        return Compare(Symbol, other.Symbol, StringComparison.Ordinal);
    }

    public override string ToString()
    {
        return $"{Symbol}:{Time:d}";
    }
};