using StockExchangeSystem.API.Models;
using System.ComponentModel.DataAnnotations;

namespace StockExchangeSystem.Tests.API.Validations
{
    public class TradeDtoTests
    {
        [Fact]
        public void TradeDto_Validation_Success()
        {
            // Arrange
            var tradeDto = new TradeDto
            {
                TickerSymbol = "AAPL",
                Price = 150.00m,
                Quantity = 10,
                BrokerId = 1
            };
            var context = new ValidationContext(tradeDto);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(tradeDto, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("", "Ticker symbol is required.")]
        [InlineData("12345678901", "Ticker symbol must not exceed 10 characters.")]
        public void TradeDto_TickerSymbol_Validation_Failures(string tickerSymbol, string expectedErrorMessage)
        {
            // Arrange
            var tradeDto = new TradeDto
            {
                TickerSymbol = tickerSymbol,
                Price = 100.00m,
                Quantity = 5,
                BrokerId = 1
            };
            var context = new ValidationContext(tradeDto);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(tradeDto, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == expectedErrorMessage);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void TradeDto_Price_Validation_Failures(decimal price)
        {
            // Arrange
            var tradeDto = new TradeDto
            {
                TickerSymbol = "AAPL",
                Price = price,
                Quantity = 5,
                BrokerId = 1
            };
            var context = new ValidationContext(tradeDto);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(tradeDto, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == "Price must be greater than 0.");
        }

        [Fact]
        public void TradeDto_Quantity_Validation_Failure()
        {
            // Arrange
            var tradeDto = new TradeDto
            {
                TickerSymbol = "AAPL",
                Price = 100.00m,
                // Invalid quantity
                Quantity = 0, 
                BrokerId = 1
            };
            var context = new ValidationContext(tradeDto);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(tradeDto, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == "Quantity must be at least 1.");
        }

        [Fact]
        public void TradeDto_BrokerId_Validation_Failure()
        {
            // Arrange
            var tradeDto = new TradeDto
            {
                TickerSymbol = "AAPL",
                Price = 100.00m,
                Quantity = 5,
                // Invalid Broker ID
                BrokerId = 0 
            };
            var context = new ValidationContext(tradeDto);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(tradeDto, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == "Broker ID must be a positive number.");
        }
    }
}
