using Microsoft.AspNetCore.Mvc;
using StockExchangeSystem.API.Models;
using StockExchangeSystem.API.Utilities.Interfaces;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // This maps to: api/Stocks 
    public class StocksController : ControllerBase
    {
        private readonly IStockRepository stockRepository;
        private readonly IPaginationHelper paginationHelper;

        public StocksController(IStockRepository _stockRepository, IPaginationHelper _paginationHelper)
        {
            stockRepository = _stockRepository;
            paginationHelper = _paginationHelper;
        }

        [HttpPost]
        public async Task<IActionResult> AddStock([FromBody] StockDto stockDto)
        {
            // Basic validation of the input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a new Stock object from the DTO
            var stock = new Stock
            {
                TickerSymbol = stockDto.TickerSymbol,
                CurrentPrice = stockDto.CurrentPrice
            };

            // Use the repository to add the new stock
            await stockRepository.AddStockAsync(stock);

            // Return a 201 Created response with the location of the newly created stock
            return CreatedAtAction(nameof(GetStockPrice), new { tickerSymbol = stock.TickerSymbol }, stock);
        }

        [HttpGet("{tickerSymbol}")]
        public async Task<IActionResult> GetStockPrice(string tickerSymbol)
        {
            var stock = await stockRepository.GetStockByTickerAsync(tickerSymbol);
            if (stock == null)
            {
                return NotFound($"Stock with ticker symbol {tickerSymbol} not found.");
            }

            return Ok(new { TickerSymbol = tickerSymbol, stock.CurrentPrice });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStocks([FromQuery] int page_size = 10, [FromQuery] int page_number = 1)
        {
            var allStocks = await stockRepository.GetAllStocksAsync();

            var pagedStocks = paginationHelper.Paginate(allStocks.AsQueryable(), page_size, page_number)
                                    .Select(stock => new { stock.TickerSymbol, stock.CurrentPrice })
                                    .ToList();

            var totalStockCount = allStocks.Count;

            return Ok(new
            {
                Stocks = pagedStocks,
                TotalResults = totalStockCount,
                PageSize = page_size,
                PageNumber = page_number
            });
        }

        [HttpPost("range")]
        public async Task<IActionResult> GetStocksByRange([FromBody] TickerSymbolsRequest request)
        {
            if (request.TickerSymbols == null || !request.TickerSymbols.Any())
            {
                return BadRequest("Ticker symbols list cannot be empty.");
            }

            var foundStocks = new List<dynamic>();

            foreach (var tickerSymbol in request.TickerSymbols)
            {
               var stock = await stockRepository.GetStockByTickerAsync(tickerSymbol);

                if (stock != null)
                {
                    // If the stock is found, add it to the found list
                    foundStocks.Add(new { stock.TickerSymbol, stock.CurrentPrice });
                }
            }

            // Prepare the response object
            return Ok(new
            {
                FoundStocks = foundStocks,
            });
        }
    }
}
