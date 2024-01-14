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
            new("XOM", Sector.Energy), new("CVX", Sector.Energy), new("KO", Sector.ConsumerDefensive),
            new("MCD", Sector.ConsumerCyclical), new("T", Sector.CommunicationServices), new("VZ", Sector.CommunicationServices),
            new("JNJ", Sector.Healthcare), new("PFE", Sector.Healthcare), new("IBM", Sector.Technology),
            new("ABBV", Sector.Healthcare), new("TGT", Sector.ConsumerDefensive), new("COP", Sector.Energy),
            new("F", Sector.ConsumerCyclical), new ("HD", Sector.ConsumerCyclical), new ("JPM", Sector.Financial),
            new ("BAC", Sector.Financial), new("SBUX", Sector.ConsumerCyclical), new("PG", Sector.ConsumerDefensive),
            new("CL",Sector.ConsumerDefensive), new("PEP", Sector.ConsumerDefensive), new("PM", Sector.ConsumerDefensive),
            new("EOG", Sector.Energy)
        };
        for (int i = 0; i < symbolSectors.Count; i++)
        {
            symbolSectors[i].Id = i + 1;
        }
        modelBuilder.Entity<SymbolSector>()
            .HasData(symbolSectors);
    }
}