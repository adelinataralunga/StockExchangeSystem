using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using StockExchangeSystem.Data.Configuration;
using StockExchangeSystem.Data;

namespace StockExchangeSystem.Tests
{
    public class TestStockExchangeDbContextFactory : IDesignTimeDbContextFactory<StockExchangeDbContext>
    {
        public StockExchangeDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StockExchangeDbContext>();
            // Use a unique name for each context instance
            var dbName = $"TestStockExchangeInMemoryDB_{Guid.NewGuid()}";
            optionsBuilder.UseInMemoryDatabase(dbName);

            return new StockExchangeDbContext(optionsBuilder.Options, new InMemoryDatabaseConfiguration());
        }
    }
}
