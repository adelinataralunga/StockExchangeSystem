using StockExchangeSystem.API.Utilities;

namespace StockExchangeSystem.Tests.API.Utilities
{
    public class PaginationHelperTests
    {
        [Fact]
        public void Paginate_ValidParameters_ReturnsCorrectPage()
        {
            // Arrange
            var helper = new PaginationHelper();
            var query = Enumerable.Range(1, 100).AsQueryable();
            int pageSize = 10;
            int pageNumber = 3;
            var expectedPage = Enumerable.Range(21, 10).AsQueryable();

            // Act
            var paginatedQuery = helper.Paginate(query, pageSize, pageNumber);

            // Assert
            Assert.Equal(expectedPage, paginatedQuery);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(-1, -1)]
        public void Paginate_InvalidParameters_ThrowsArgumentException(int pageSize, int pageNumber)
        {
            // Arrange
            var helper = new PaginationHelper();
            var query = Enumerable.Empty<int>().AsQueryable();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => helper.Paginate(query, pageSize, pageNumber));
        }

        [Fact]
        public void Paginate_PageNumberExceeds_ReturnsEmpty()
        {
            // Arrange
            var helper = new PaginationHelper();
            var query = Enumerable.Range(1, 15).AsQueryable();
            int pageSize = 10;
            int pageNumber = 3; // No items should be in the third page

            // Act
            var paginatedQuery = helper.Paginate(query, pageSize, pageNumber);

            // Assert
            Assert.Empty(paginatedQuery);
        }
    }
}
