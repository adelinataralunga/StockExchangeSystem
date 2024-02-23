using Microsoft.AspNetCore.Mvc;
using Moq;
using StockExchangeSystem.API.Controllers;
using StockExchangeSystem.API.Models;
using StockExchangeSystem.API.Utilities.Interfaces;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Tests.API.Controllers
{
    public class BrokersControllerTests
    {
        private readonly Mock<IBrokerRepository> mockBrokerRepository = new();
        private readonly Mock<IPaginationHelper> mockPaginationHelper = new();
        private readonly BrokersController sut;

        public BrokersControllerTests()
        {
            sut = new BrokersController(mockBrokerRepository.Object, mockPaginationHelper.Object);
        }

        [Fact]
        public async Task GetAllBrokers_ReturnsPagedBrokers()
        {
            // Arrange
            var brokers = new List<Broker>
            {
                new Broker { Id = 1, Name = "Broker1" },
                new Broker { Id = 2, Name = "Broker2" }
            };
            mockBrokerRepository.Setup(repo => repo.GetAllBrokersAsync()).ReturnsAsync(brokers);
            mockPaginationHelper.Setup(p => p.Paginate(It.IsAny<IQueryable<Broker>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(brokers.AsQueryable());

            // Act
            var result = await sut.GetAllBrokers(10, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task AddBroker_ValidBroker_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var brokerDto = new BrokerDto { Name = "New Broker" };

            // Act
            var result = await sut.AddBroker(brokerDto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task GetBrokerById_BrokerExists_ReturnsOkObjectResult()
        {
            // Arrange
            var broker = new Broker { Id = 1, Name = "Existing Broker" };
            mockBrokerRepository.Setup(repo => repo.GetBrokerByIdAsync(1)).ReturnsAsync(broker);

            // Act
            var result = await sut.GetBrokerById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultBroker = Assert.IsType<Broker>(okResult.Value);
            Assert.Equal("Existing Broker", resultBroker.Name);
        }
    }
}