using Microsoft.EntityFrameworkCore;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Data
{
    public class StockExchangeDbContext : DbContext
    {
        private readonly IDatabaseConfiguration _databaseConfiguration;

        public StockExchangeDbContext(DbContextOptions<StockExchangeDbContext> options,
            IDatabaseConfiguration databaseConfiguration) : base(options)
        {
            _databaseConfiguration = databaseConfiguration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(_databaseConfiguration.GetConnectionString());
            }
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Broker> Brokers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Entities for Auto-Increment IDs
            modelBuilder.Entity<Stock>().HasKey(s => s.Id);
            modelBuilder.Entity<Trade>().HasKey(t => t.Id);
            modelBuilder.Entity<Broker>().HasKey(b => b.Id);

            // Seed Stocks
            modelBuilder.Entity<Stock>().HasData(
                new Stock { Id = 1, TickerSymbol = "AAPL", CurrentPrice = 150 },
                new Stock { Id = 2, TickerSymbol = "MSFT", CurrentPrice = 250 },
                new Stock { Id = 3, TickerSymbol = "XOM", CurrentPrice = 200 },
                new Stock { Id = 4, TickerSymbol = "GE", CurrentPrice = 90 },
                new Stock { Id = 5, TickerSymbol = "GOOGL", CurrentPrice = 500 },
                new Stock { Id = 6, TickerSymbol = "TSLA", CurrentPrice = 320 }
            );

            // Seed Brokers
            modelBuilder.Entity<Broker>().HasData(
                new Broker { Id = 1,Name = "Broker A" },
                new Broker { Id = 2, Name = "Broker B" }
            );

            // Seed Trades 
            modelBuilder.Entity<Trade>().HasData(
                new Trade { Id = 1, StockId = 1, Price = 145, Quantity = 5, BrokerId = 1, Timestamp = DateTime.UtcNow.AddDays(-1) },
                new Trade { Id = 2, StockId = 2, Price = 260, Quantity = 2, BrokerId = 2, Timestamp = DateTime.UtcNow }
            );
        }
    }
}
