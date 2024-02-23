using Moq;
using StockExchangeSystem.API.Services;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Tests.API.Services
{
    public class TradeGeneratorServiceTests
    {
        private readonly Mock<IStockRepository> mockStockRepository = new();
        private readonly Mock<ITradeRepository> mockTradeRepository = new();
        private readonly TradeGeneratorService sut;

        public TradeGeneratorServiceTests()
        {
            sut = new TradeGeneratorService(mockStockRepository.Object, mockTradeRepository.Object);
        }

        [Fact]
        public async Task GenerateTradesAsync_GeneratesTradesWithinDuration()
        {
            // Arrange
            var duration = TimeSpan.FromSeconds(5); // Use a short, controlled duration for the test
            mockStockRepository.Setup(repo => repo.GetStockByTickerAsync(It.IsAny<string>()))
                .ReturnsAsync(new Stock { Id = 1, CurrentPrice = 100 });
            mockTradeRepository.Setup(repo => repo.AddTradeAsync(It.IsAny<Trade>()))
                .Returns(Task.CompletedTask);

            // Act
            int tradesGenerated = await sut.GenerateTradesAsync(duration);

            // Assert
            Assert.True(tradesGenerated > 0, "Expected to generate at least one trade.");
            mockTradeRepository.Verify(repo => repo.AddTradeAsync(It.IsAny<Trade>()), Times.AtLeastOnce());
        }

        [Fact]
        public async Task GenerateTradesAsync_UpdatesStockPriceAfterTrade()
        {
            // Arrange
            var stock = new Stock { Id = 1, CurrentPrice = 100 };
            mockStockRepository.Setup(repo => repo.GetStockByTickerAsync(It.IsAny<string>()))
                .ReturnsAsync(stock);
            mockTradeRepository.Setup(repo => repo.AddTradeAsync(It.IsAny<Trade>()))
                .Returns(Task.CompletedTask);
            mockStockRepository.Setup(repo => repo.UpdateStockPriceAsync(It.IsAny<Stock>()))
                .Returns(Task.CompletedTask);

            // Act
            await sut.GenerateTradesAsync(TimeSpan.FromSeconds(1));

            // Assert
            mockStockRepository.Verify(repo => repo.UpdateStockPriceAsync(It.Is<Stock>(s => s.CurrentPrice != 100)), Times.AtLeastOnce());
        }
    }
}
