using StockExchangeSystem.Data.Interfaces;

namespace StockExchangeSystem.Data.Configuration
{
    public class TestingDatabaseConfiguration : IDatabaseConfiguration
    {
        public string GetConnectionString() =>
            "Filename=../StockExchangeSystem.Data/StockExchange.db";
    }
}
