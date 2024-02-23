using StockExchangeSystem.API.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockExchangeSystem.Tests.API.Validations
{
    public class TickerSymbolsRequestTests
    {
        [Fact]
        public void TickerSymbolsRequest_RequiredAttribute_TriggersForEmptyList()
        {
            // Arrange
            var model = new TickerSymbolsRequest
            {
                TickerSymbols = new List<string>() 
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "Ticker symbols list cannot be empty.");

        }

        [Fact]
        public void TickerSymbolsRequest_ValidTickerSymbols_PassesValidation()
        {
            // Arrange
            var model = new TickerSymbolsRequest
            {
                TickerSymbols = new List<string> { "AAPL", "MSFT" }
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.DoesNotContain(validationResults, vr => vr.ErrorMessage == "Invalid ticker symbols.");
        }

        [Fact]
        public void TickerSymbolsRequest_InvalidTickerSymbols_FailsValidation()
        {
            // Arrange
            var model = new TickerSymbolsRequest
            {
                TickerSymbols = new List<string> { "INVALID1", "TOOLONGTICKERSYMBOL" }
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "Ticker symbols list contains invalid symbols. Each symbol must be non-empty, alphanumeric, and up to 5 characters long.");
        }
    }
}
