using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using StockExchangeSystem.Data.Configuration;

namespace StockExchangeSystem.Data
{
    public class StockExchangeDbContextFactory : IDesignTimeDbContextFactory<StockExchangeDbContext>
    {
        public StockExchangeDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StockExchangeDbContext>();

            // Use a test database for the factory by default
            optionsBuilder.UseSqlite("Filename=../StockExchangeSystem.Data/StockExchange.db");

            return new StockExchangeDbContext(optionsBuilder.Options, new TestingDatabaseConfiguration());
        }
    }
}