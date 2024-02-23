using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Tests.Domain
{
    public class TradeTests
    {
        [Fact]
        public void IsValid_WithPositivePriceAndQuantity_ReturnsTrue()
        {
            var trade = new Trade
            {
                Price = 100m,
                Quantity = 5m,
                BrokerId = 1
            };

            bool result = trade.IsValid();

            Assert.True(result);
        }

        [Theory]
        [InlineData(-1, 5, 1)] // Negative price
        [InlineData(100, 0, 1)] // Zero quantity
        [InlineData(100, -5, 1)] // Negative quantity
        [InlineData(100, 5, 0)] // Invalid BrokerId
        public void IsValid_WithInvalidInputs_ReturnsFalse(decimal price, decimal quantity, int brokerId)
        {
            // Arrange
            var trade = new Trade
            {
                Price = price,
                Quantity = quantity,
                BrokerId = brokerId
            };

            // Act
            bool result = trade.IsValid();

            // Assert
            Assert.False(result);
        }
    }
}
