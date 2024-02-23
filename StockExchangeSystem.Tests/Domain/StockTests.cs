using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Tests.Domain
{
    public class StockTests
    {
        [Fact]
        public void IsValid_WithValidTickerSymbolAndPrice_ReturnsTrue()
        {
            // Arrange
            var stock = new Stock
            {
                TickerSymbol = "AAPL",
                CurrentPrice = 150m
            };

            // Act
            bool result = stock.IsValid();

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("", 100)] // Empty ticker symbol
        [InlineData(" ", 100)] // Whitespace ticker symbol
        [InlineData("AAPL1!", 100)] // Invalid characters in ticker symbol
        [InlineData("AAPL", -1)] // Negative price
        public void IsValid_WithInvalidTickerSymbolOrPrice_ReturnsFalse(string tickerSymbol, decimal currentPrice)
        {
            // Arrange
            var stock = new Stock
            {
                TickerSymbol = tickerSymbol,
                CurrentPrice = currentPrice
            };

            // Act
            bool result = stock.IsValid();

            // Assert
            Assert.False(result);
        }
    }
}
