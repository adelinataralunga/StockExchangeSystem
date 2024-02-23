using StockExchangeSystem.Data;
using StockExchangeSystem.Data.Repositories;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Tests.Data
{
    public class BrokerRepositoryTests
    {
        private static StockExchangeDbContext GetDbContext()
        {
            var factory = new TestStockExchangeDbContextFactory();
            var dbContext = factory.CreateDbContext(Array.Empty<string>());
            return dbContext;
        }

        [Fact]
        public async Task AddBrokerAsync_AddsBrokerSuccessfully()
        {
            // Arrange
            using var dbContext = GetDbContext();
            var repository = new BrokerRepository(dbContext);

            var newBroker = new Broker { Name = "Test Broker" };

            // Act
            var addedBroker = await repository.AddBrokerAsync(newBroker);

            // Assert
            // addedBroker has an Id (it was saved to the database)
            Assert.NotEqual(0, addedBroker.Id);
            Assert.Equal("Test Broker", addedBroker.Name);

            // Assert the broker is actually in the database
            var retrievedBroker = await dbContext.Brokers.FindAsync(addedBroker.Id);
            Assert.NotNull(retrievedBroker);
        }

        [Fact]
        public async Task GetAllBrokersAsync_ReturnsAllBrokers()
        {
            // Arrange
            using var dbContext = GetDbContext();
            // Pre-seed the database if needed for this test
            dbContext.Brokers.AddRange(
                new Broker { Name = "Broker 1" },
                new Broker { Name = "Broker 2" }
            );
            await dbContext.SaveChangesAsync();

            var repository = new BrokerRepository(dbContext);

            // Act
            var brokers = await repository.GetAllBrokersAsync();


            // Assert
            Assert.Equal(2, brokers.Count);
        }

        [Fact]
        public async Task GetBrokerByIdAsync_ReturnsBrokerWhenExists()
        {
            // Arrange
            using var context = GetDbContext();

            // Seed database with a test broker
            context.Brokers.Add(new Broker { Id = 1, Name = "Test Broker" });
            await context.SaveChangesAsync();

            var repository = new BrokerRepository(context);

            // Act
            var broker = await repository.GetBrokerByIdAsync(1); // Assuming the test broker has Id = 1

            // Assert
            Assert.NotNull(broker);
            Assert.Equal(1, broker.Id);
            Assert.Equal("Test Broker", broker.Name);
        }

        [Fact]
        public async Task GetBrokerByIdAsync_ThrowsBrokerNotFoundExceptionWhenNotExists()
        {
            // Arrange
            using var context = GetDbContext();
            var repository = new BrokerRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<BrokerNotFoundException>(async () => await repository.GetBrokerByIdAsync(999));
        }
    }
}
