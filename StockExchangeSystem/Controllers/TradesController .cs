using Microsoft.AspNetCore.Mvc;
using StockExchangeSystem.API.Models;
using StockExchangeSystem.API.Services.Interfaces;
using StockExchangeSystem.API.Utilities.Interfaces;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // This maps to: api/Trade
    public class TradesController : ControllerBase
    {
        private readonly ITradeRepository tradeRepository;
        private readonly IStockRepository stockRepository;
        private readonly ITradeGeneratorService tradeGeneratorService;
        private readonly IPaginationHelper paginationHelper;

        public TradesController(ITradeRepository _tradeRepository, IStockRepository _stockRepository, 
            ITradeGeneratorService _tradeGeneratorService, IPaginationHelper _paginationHelper)
        {
            tradeRepository = _tradeRepository;
            stockRepository = _stockRepository;
            tradeGeneratorService = _tradeGeneratorService;
            paginationHelper = _paginationHelper;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveTrade([FromBody] TradeDto tradeDto)
        {
            //  Basic Validation 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the stock 
            var stock = await stockRepository.GetStockByTickerAsync(tradeDto.TickerSymbol) 
                ?? throw new StockNotFoundException("Stock not found");

            // Create Trade object and update price
            var trade = new Trade
            {
                StockId = stock.Id,
                Price = tradeDto.Price,
                Quantity = tradeDto.Quantity,
                BrokerId = tradeDto.BrokerId,
                Timestamp = DateTime.UtcNow
            };

            // Persist the Trade, adding it to the repo
            await tradeRepository.AddTradeAsync(trade);
            // Update the stock price
            stock.CurrentPrice = tradeDto.Price;
            await stockRepository.UpdateStockPriceAsync(stock);

            return CreatedAtAction(nameof(GetTradeByIdAsync), new { id = trade.Id }, trade);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTradeByIdAsync(int id)
        {
            var trade = await tradeRepository.GetTradeByIdAsync(id);
            if (trade == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                trade.Id,
                trade.StockId,
                TickerSymbol = trade.Stock?.TickerSymbol ?? "Unknown",
                trade.Price,
                trade.Quantity,
                trade.BrokerId,
                trade.Broker?.Name,
                trade.Timestamp
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrades([FromQuery] int page_size = 10, [FromQuery] int page_number = 1)
        {
            var allTrades = await tradeRepository.GetAllTradesAsync();
            var pagedTrades = paginationHelper.Paginate(allTrades.AsQueryable(), page_size, page_number)
                .Select(trade => new {
                    trade.Id,
                    trade.StockId,
                    TickerSymbol = trade.Stock != null ? trade.Stock.TickerSymbol : "Unknown",
                    trade.Price,
                    trade.Quantity,
                    trade.BrokerId,
                    trade.Timestamp
                })
                .ToList();

            var totalTradeCount = allTrades.Count;

            return Ok(new
            {
                Trades = pagedTrades,
                TotalResults = totalTradeCount,
                PageSize = page_size,
                PageNumber = page_number
            });
            
           
        }

        [HttpPost("generate-trades")]
        public async Task<IActionResult> StartTradeGeneration([FromQuery] int durationSeconds)
        {
            var duration = TimeSpan.FromSeconds(durationSeconds);

            int tradesGenerated = await tradeGeneratorService.GenerateTradesAsync(duration);

            if (tradesGenerated == 0)
            {
                return BadRequest("No trades generated due to missing stocks.");
            }
            else
            {
                return Ok(new { Message = "Trade generation completed.", TradesGenerated = tradesGenerated });
            }
        }
    }
}
