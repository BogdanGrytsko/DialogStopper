using static System.String;

namespace Trader;

public record SymbolTime(string Symbol, DateTime Time) : IComparable<SymbolTime>
{
    public int CompareTo(SymbolTime? other)
    {
        if (other == null)
            throw new NotImplementedException();
        if (other.Symbol != Symbol)
            return Compare(Symbol, other.Symbol, StringComparison.Ordinal);
        return Time.CompareTo(other.Time);
    }

    public override string ToString()
    {
        return $"{Symbol}:{Time:d}";
    }
};