using StockExchangeSystem.Data.Interfaces;

namespace StockExchangeSystem.Data.Configuration
{
    public class InMemoryDatabaseConfiguration : IDatabaseConfiguration
    {
        public string GetConnectionString() => "Data Source=:memory:";
    }
}
