using Microsoft.AspNetCore.Mvc;
using Moq;
using StockExchangeSystem.API.Models;
using StockExchangeSystem.API.Services.Interfaces;
using StockExchangeSystem.API.Utilities.Interfaces;
using StockExchangeSystem.Controllers;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Tests.API.Controllers
{
    public class TradesControllerTests
    {
        private readonly Mock<ITradeRepository> mockTradeRepository = new();
        private readonly Mock<IStockRepository> mockStockRepository = new();
        private readonly Mock<ITradeGeneratorService> mockTradeGeneratorService = new();
        private readonly Mock<IPaginationHelper> mockPaginationHelper = new();
        private readonly TradesController sut;

        public TradesControllerTests()
        {
            sut = new TradesController(
                mockTradeRepository.Object,
                mockStockRepository.Object,
                mockTradeGeneratorService.Object,
                mockPaginationHelper.Object);
        }

        [Fact]
        public async Task ReceiveTrade_ValidTrade_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var tradeDto = new TradeDto { TickerSymbol = "AAPL", Price = 150, Quantity = 10, BrokerId = 1 };
            var stock = new Stock { Id = 1, TickerSymbol = "AAPL", CurrentPrice = 145 };
            mockStockRepository.Setup(repo => repo.GetStockByTickerAsync("AAPL")).ReturnsAsync(stock);
            mockTradeRepository.Setup(repo => repo.AddTradeAsync(It.IsAny<Trade>())).Returns(Task.CompletedTask);

            // Act
            var result = await sut.ReceiveTrade(tradeDto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task GetTradeByIdAsync_ValidId_ReturnsOkObjectResult()
        {
            // Arrange
            var trade = new Trade { Id = 1, StockId = 1, Price = 100, Quantity = 5, BrokerId = 1, Timestamp = DateTime.UtcNow };
            mockTradeRepository.Setup(repo => repo.GetTradeByIdAsync(1)).ReturnsAsync(trade);

            // Act
            var result = await sut.GetTradeByIdAsync(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAllTrades_ReturnsPagedResults()
        {
            // Arrange
            var trades = Enumerable.Range(1, 20).Select(i => new Trade { Id = i }).ToList();
            mockTradeRepository.Setup(repo => repo.GetAllTradesAsync()).ReturnsAsync(trades);
            mockPaginationHelper.Setup(p => p.Paginate(It.IsAny<IQueryable<Trade>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(trades.Take(10).AsQueryable());

            // Act
            var result = await sut.GetAllTrades(10, 1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var value = okResult.Value;
            var tradesProperty = value?.GetType().GetProperty("Trades");
            Assert.NotNull(tradesProperty);
            var tradesValue = tradesProperty.GetValue(value) as IEnumerable<dynamic>;
            Assert.Equal(10, tradesValue?.Count());

        }

        [Fact]
        public async Task StartTradeGeneration_WithMissingStocks_ReturnsBadRequest()
        {
            // Arrange
            mockTradeGeneratorService.Setup(s => s.GenerateTradesAsync(It.IsAny<TimeSpan>())).ReturnsAsync(0);

            // Act
            // Run for 10 seconds
            var result = await sut.StartTradeGeneration(10);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value); // Ensure the Value is not null

            var message = badRequestResult.Value.ToString();
            Assert.Contains("No trades generated due to missing stocks", message);
        }

        [Fact]
        public async Task StartTradeGeneration_SuccessfulTradeGeneration_ReturnsOkObjectResult()
        {
            // Arrange
            // Simulate 5 trades generated, no missing stocks
            mockTradeGeneratorService.Setup(s => s.GenerateTradesAsync(It.IsAny<TimeSpan>()))
                .ReturnsAsync(5);

            // Act
            // Run for 10 seconds
            var result = await sut.StartTradeGeneration(10);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var valueType = okResult?.Value?.GetType();
            var tradesGeneratedProperty = valueType?.GetProperty("TradesGenerated");
            Assert.NotNull(tradesGeneratedProperty);
            var tradesGeneratedValue = tradesGeneratedProperty.GetValue(okResult?.Value);
            Assert.Equal(5, tradesGeneratedValue);
        }
    }
}


