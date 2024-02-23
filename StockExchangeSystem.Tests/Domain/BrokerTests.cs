using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Tests.Domain
{
    public class BrokerTests
    {
        [Fact]
        public void IsValid_WithName_ReturnsTrue()
        {
            // Arrange
            var broker = new Broker { Name = "Valid Broker Name" };

            // Act
            bool result = broker.IsValid();

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void IsValid_WithInvalidName_ReturnsFalse(string name)
        {
            // Arrange
            var broker = new Broker { Name = name };

            // Act
            bool result = broker.IsValid();

            // Assert
            Assert.False(result);
        }
    }
}
