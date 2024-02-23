using Microsoft.AspNetCore.Mvc;
using StockExchangeSystem.API.Models;
using StockExchangeSystem.API.Utilities.Interfaces;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrokersController : ControllerBase
    {
        private readonly IBrokerRepository brokerRepository;
        private readonly IPaginationHelper paginationHelper;

        public BrokersController(IBrokerRepository _brokerRepository, IPaginationHelper _paginationHelper)
        {
            brokerRepository = _brokerRepository;
            paginationHelper = _paginationHelper;
        }

        // GET: api/Brokers?page_size=10&page_number=1
        [HttpGet]
        public async Task<IActionResult> GetAllBrokers([FromQuery] int page_size = 10, [FromQuery] int page_number = 1)
        {
            var brokersQuery = await brokerRepository.GetAllBrokersAsync();

            // Apply pagination
            var pagedBrokers = paginationHelper.Paginate(brokersQuery.AsQueryable(), page_size, page_number)
                .ToList();

            var totalBrokersCount = brokersQuery.Count;

            return Ok(new
            {
                Brokers = pagedBrokers,
                TotalResults = totalBrokersCount,
                PageSize = page_size,
                PageNumber = page_number
            });
        }

        // POST: api/Brokers
        [HttpPost]
        public async Task<IActionResult> AddBroker([FromBody] BrokerDto brokerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a new Broker object from the DTO
            var broker = new Broker
            {
                Name = brokerDto.Name
            };

            await brokerRepository.AddBrokerAsync(broker);
            return CreatedAtAction(nameof(GetBrokerById), new { broker });
        }

        // GET: api/Brokers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrokerById(int id)
        {
            var broker = await brokerRepository.GetBrokerByIdAsync(id);

            if (broker == null)
            {
                return NotFound();
            }

            return Ok(broker);
        }
    }
}
