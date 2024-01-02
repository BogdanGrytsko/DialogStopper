using Microsoft.EntityFrameworkCore;
using Trader.Dividends;

namespace Trader;

public class TradingContext : DbContext
{
    public DbSet<Candle> Candles { get; set; }
    public DbSet<Dividend> Dividends { get; set; }
    public DbSet<SymbolSector> SymbolSectors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=.;Database=Trading;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Candle>()
            .HasIndex(p => new { p.Symbol, p.Date }).IsUnique();
        modelBuilder.Entity<Dividend>()
            .HasIndex(p => new { p.Symbol, p.ExDate }).IsUnique();
        modelBuilder.Entity<SymbolSector>()
            .HasIndex(p => new { p.Symbol }).IsUnique();
        var symbolSectors = new List<SymbolSector>
        {
            new("XOM", "Energy"), new("CVX", "Energy"), new("KO", "Consumer Defensive"),
            new("MCD", "Consumer cyclical"), new("T", "Communication services"), new("VZ", "Communication services"),
            new("JNJ", "Healthcare"), new("PFE", "Healthcare"), new("IBM", "Technology"),
            new("ABBV", "Healthcare"), new("TGT", "Consumer Defensive"), new("COP", "Energy"),
            new("F", "Consumer cyclical"), new ("HD", "Consumer cyclical"), new ("JPM", "Financial"),
            new ("BAC", "Financial"), new("SBUX", "Consumer cyclical"), new("PG", "Consumer Defensive"),
            new("CL","Consumer Defensive"), new("PEP", "Consumer Defensive"), new("PM", "Consumer Defensive")
        };
        for (int i = 0; i < symbolSectors.Count; i++)
        {
            symbolSectors[i].Id = i + 1;
        }
        modelBuilder.Entity<SymbolSector>()
            .HasData(symbolSectors);
    }
}