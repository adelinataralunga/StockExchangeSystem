using StockExchangeSystem.Data;
using StockExchangeSystem.Data.Repositories;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Tests.Data
{
    public class StockRepositoryTests
    {
        private static StockExchangeDbContext GetDbContext()
        {
            var factory = new TestStockExchangeDbContextFactory();
            var dbContext = factory.CreateDbContext(Array.Empty<string>());
            return dbContext;
        } 

        private static async Task SeedDataAsync(StockExchangeDbContext context)
        {
            context.Stocks.AddRange(
                new Stock { Id = 1, TickerSymbol = "AAPL", CurrentPrice = 1500 },
                new Stock { Id = 2, TickerSymbol = "MSFT", CurrentPrice = 2505 }
            );
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllStocksAsync_ReturnsAllStocks()
        {
            // Arrange
            using var context = GetDbContext();
            await SeedDataAsync(context);
            var repository = new StockRepository(context);

            // Act
            var stocks = await repository.GetAllStocksAsync();

            // Assert
            // Added 2 stocks in the SeedDataAsync method
            Assert.Equal(2, stocks.Count);
        }

        [Fact]
        public async Task GetStockByTickerAsync_ReturnsStockWhenExists()
        {
            // Arrange
            using var context = GetDbContext();
            await SeedDataAsync(context);
            var repository = new StockRepository(context);

            // Act
            var stock = await repository.GetStockByTickerAsync("AAPL");

            // Assert
            Assert.NotNull(stock);
            Assert.Equal("AAPL", stock.TickerSymbol);
        }

        [Fact]
        public async Task GetStockByTickerAsync_ThrowsStockNotFoundExceptionWhenNotExists()
        {
            // Arrange
            using var context = GetDbContext();
            await SeedDataAsync(context);
            var repository = new StockRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<StockNotFoundException>(async () => await repository.GetStockByTickerAsync("GOOG"));
        }

        [Fact]
        public async Task UpdateStockPriceAsync_UpdatesPriceSuccessfully()
        {
            // Arrange
            using var context = GetDbContext();
            await SeedDataAsync(context);
            var repository = new StockRepository(context);

            var existingStock = await context.Stocks.FindAsync(1);

            // Act
            if (existingStock != null)
            {
                existingStock.CurrentPrice = 160;
                await repository.UpdateStockPriceAsync(existingStock);
            }

            // Assert
            var updatedStock = await context.Stocks.FindAsync(1);
            Assert.Equal(160, updatedStock?.CurrentPrice);
        }

        [Fact]
        public async Task AddStockAsync_AddsStockSuccessfully()
        {
            // Arrange
            using var context = GetDbContext();
            var repository = new StockRepository(context);

            var newStock = new Stock { Id = 3, TickerSymbol = "GOOG", CurrentPrice = 1200 };

            // Act
            await repository.AddStockAsync(newStock);

            // Assert
            var addedStock = await context.Stocks.FindAsync(3);
            Assert.NotNull(addedStock);
            Assert.Equal("GOOG", addedStock.TickerSymbol);
        }

        [Fact]
        public async Task AddStockAsync_ThrowsArgumentExceptionWhenTickerExists()
        {
            // Arrange
            using var context = GetDbContext();
            await SeedDataAsync(context);
            var repository = new StockRepository(context);

            var duplicateStock = new Stock { Id = 3, TickerSymbol = "AAPL", CurrentPrice = 1200 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await repository.AddStockAsync(duplicateStock));
        }
    }
}