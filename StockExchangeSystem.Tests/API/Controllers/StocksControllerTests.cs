using Microsoft.AspNetCore.Mvc;
using Moq;
using StockExchangeSystem.API.Models;
using StockExchangeSystem.API.Utilities.Interfaces;
using StockExchangeSystem.Controllers;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Tests.API.Controllers
{
    public class StocksControllerTests
    {
        private readonly Mock<IStockRepository> mockStockRepository = new();
        private readonly Mock<IPaginationHelper> mockPaginationHelper = new();
        private readonly StocksController sut;

        public StocksControllerTests()
        {
            sut = new StocksController(mockStockRepository.Object, mockPaginationHelper.Object);
        }

        [Fact]
        public async Task AddStock_ValidStock_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var stockDto = new StockDto { TickerSymbol = "AAPL", CurrentPrice = 150 };

            // Act
            var result = await sut.AddStock(stockDto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task GetStockPrice_StockExists_ReturnsOkObjectResult()
        {
            // Arrange
            var tickerSymbol = "AAPL";
            var stock = new Stock { TickerSymbol = tickerSymbol, CurrentPrice = 150 };
            mockStockRepository.Setup(repo => repo.GetStockByTickerAsync(tickerSymbol)).ReturnsAsync(stock);

            // Act
            var result = await sut.GetStockPrice(tickerSymbol);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAllStocks_ReturnsPagedStocks()
        {
            // Arrange
            var stocks = Enumerable.Range(1, 20).Select(i => new Stock { TickerSymbol = $"AAPL{i}", CurrentPrice = 150 + i }).ToList();
            mockStockRepository.Setup(repo => repo.GetAllStocksAsync()).ReturnsAsync(stocks);
            mockPaginationHelper.Setup(p => p.Paginate(It.IsAny<IQueryable<Stock>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(stocks.Take(10).AsQueryable());

            // Act
            var result = await sut.GetAllStocks(10, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetStocksByRange_ValidRequest_ReturnsOkObjectResult()
        {
            // Arrange
            var request = new TickerSymbolsRequest { TickerSymbols = new List<string> { "AAPL", "MSFT" } };
            var stocks = new List<Stock>
            {
                new Stock { TickerSymbol = "AAPL", CurrentPrice = 150 },
                new Stock { TickerSymbol = "MSFT", CurrentPrice = 200 }
            };
            mockStockRepository.SetupSequence(repo => repo.GetStockByTickerAsync(It.IsAny<string>()))
                .ReturnsAsync(stocks[0])
                .ReturnsAsync(stocks[1]);

            // Act
            var result = await sut.GetStocksByRange(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult?.Value);
        }
    }
}
