using Microsoft.EntityFrameworkCore;
using StockExchangeSystem.Data;
using StockExchangeSystem.Data.Repositories;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Tests.Data
{
    public class TradeRepositoryTests
    {
        private static StockExchangeDbContext GetDbContext()
        {
            var factory = new TestStockExchangeDbContextFactory();
            var dbContext = factory.CreateDbContext(Array.Empty<string>());
            return dbContext;
        }

        private static async Task SeedDataAsync(StockExchangeDbContext context)
        {
            // Seed brokers
            context.Brokers.AddRange(
                new Broker { Id = 1, Name = "Broker A" }
            );

            // Seed stocks
            context.Stocks.AddRange(
                new Stock { Id = 1, TickerSymbol = "AAPL", CurrentPrice = 150 },
                new Stock { Id = 2, TickerSymbol = "MSFT", CurrentPrice = 250 }
            );

            // Seed some test data
            context.Trades.AddRange(
                new Trade { StockId = 1, Price = 100m, Quantity = 10m, BrokerId = 1, Timestamp = DateTime.UtcNow },
                new Trade { StockId = 2, Price = 200m, Quantity = 5m, BrokerId = 1, Timestamp = DateTime.UtcNow.AddDays(-1) }
            );
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddTradeAsync_AddsTradeSuccessfully()
        {
            // Arrange
            using var context = GetDbContext();
            var repository = new TradeRepository(context);
            var newTrade = new Trade { StockId = 1, Price = 120m, Quantity = 5m, BrokerId = 1, Timestamp = DateTime.UtcNow };

            // Act
            await repository.AddTradeAsync(newTrade);
            var addedTrade = await context.Trades.FirstOrDefaultAsync(t => t.Price == 120m);

            // Assert
            Assert.NotNull(addedTrade);
            Assert.Equal(120m, addedTrade.Price);
            Assert.Equal(5m, addedTrade.Quantity);
        }

        [Fact]
        public async Task GetTradeByIdAsync_ReturnsTradeWhenExists()
        {
            // Arrange
            using var context = GetDbContext();
            await SeedDataAsync(context);
            var repository = new TradeRepository(context);

            // Act
            var trade = await repository.GetTradeByIdAsync(1);

            // Assert
            Assert.NotNull(trade);
            Assert.Equal(1, trade.Id);
        }

        [Fact]
        public async Task GetTradeByIdAsync_ThrowsTradeNotFoundExceptionWhenNotExists()
        {
            // Arrange
            using var context = GetDbContext();
            // Seed has no trade with ID 999
            await SeedDataAsync(context); 
            var repository = new TradeRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<TradeNotFoundException>(async () => await repository.GetTradeByIdAsync(999));
        }

        [Fact]
        public async Task GetAllTradesAsync_ReturnsAllTradesFromDatabase()
        {
            // Arrange
            using var context = GetDbContext();
            await SeedDataAsync(context);
            var repository = new TradeRepository(context);

            // Act
            var allTrades = await repository.GetAllTradesAsync();

            // Assert
            // Added 2 trades in SeedDataAsync
            Assert.Equal(2, allTrades.Count); 
            Assert.Contains(allTrades, t => t.Stock?.TickerSymbol == "AAPL");
            Assert.Contains(allTrades, t => t.Stock?.TickerSymbol == "MSFT");

            // Checking Trade properites values as due diligence
            var aaplTrade = allTrades.FirstOrDefault(t => t.Stock?.TickerSymbol == "AAPL");
            Assert.NotNull(aaplTrade);
            Assert.Equal(100m, aaplTrade.Price);
            Assert.Equal(10m, aaplTrade.Quantity);

            var msftTrade = allTrades.FirstOrDefault(t => t.Stock?.TickerSymbol == "MSFT");
            Assert.NotNull(msftTrade);
            Assert.Equal(200m, msftTrade.Price);
            Assert.Equal(5m, msftTrade.Quantity);
        }
    }
}
