using StockExchangeSystem.API.Models;
using System.ComponentModel.DataAnnotations;

namespace StockExchangeSystem.Tests.API.Validations
{
    public class StockDtoTests
    {
        [Fact]
        public void StockDto_Validation_Success()
        {
            // Arrange
            var stockDto = new StockDto
            {
                TickerSymbol = "AAPL",
                CurrentPrice = 150.50m
            };
            var context = new ValidationContext(stockDto);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(stockDto, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("", "Ticker symbol is required.")]
        [InlineData("12345678901", "Ticker symbol must not exceed 10 characters.")]
        public void StockDto_TickerSymbol_Validation_Failures(string tickerSymbol, string expectedErrorMessage)
        {
            // Arrange
            var stockDto = new StockDto
            {
                TickerSymbol = tickerSymbol,
                CurrentPrice = 100.00m
            };
            var context = new ValidationContext(stockDto);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(stockDto, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == expectedErrorMessage);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void StockDto_CurrentPrice_Validation_Failures(decimal price)
        {
            // Arrange
            var stockDto = new StockDto
            {
                TickerSymbol = "AAPL",
                CurrentPrice = price
            };
            var context = new ValidationContext(stockDto);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(stockDto, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == "Current price must be greater than 0.");
        }
    }
}