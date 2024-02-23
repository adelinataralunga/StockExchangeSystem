using StockExchangeSystem.API.Models;
using System.ComponentModel.DataAnnotations;

namespace StockExchangeSystem.Tests.API.Validations
{
    public class BrokerDtoTests
    {
        [Fact]
        public void Validate_ModelIsValid_ReturnsSuccess()
        {
            // Arrange
            var model = new BrokerDto { Name = "Valid Broker Name" };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Validate_MissingName_ReturnsValidationError()
        {
            // Arrange
            var model = new BrokerDto { Name = "" };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "Broker name is required.");
        }

        [Fact]
        public void Validate_NameExceedsMaxLength_ReturnsValidationError()
        {
            // Arrange
            // 101 characters
            var model = new BrokerDto { Name = new string('a', 101) }; 
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage == "Broker name must not exceed 100 characters.");
        }
    }
}
