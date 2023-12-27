using Microsoft.EntityFrameworkCore;
using Trader.Dividends;

namespace Trader;

public class TradingContext : DbContext
{
    public DbSet<Candle> Candles { get; set; }
    public DbSet<Dividend> Dividends { get; set; }

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
    }
}