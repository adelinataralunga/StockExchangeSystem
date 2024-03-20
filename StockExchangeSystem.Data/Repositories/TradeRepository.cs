using Microsoft.EntityFrameworkCore;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Data.Repositories
{
    public class TradeRepository : ITradeRepository
    {
        private readonly StockExchangeDbContext dbContext;

        public TradeRepository(StockExchangeDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task AddTradeAsync(Trade trade)
        {
            await dbContext.Trades.AddAsync(trade);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Trade> GetTradeByIdAsync(int id)
        {
            // Eagerly load the Stock navigation property, by joining the Trades table with the Stocks table
            var trade = await dbContext.Trades
                .Include(t => t.Stock) 
                .Include(b => b.Broker)
                .FirstOrDefaultAsync(t => t.Id == id) ?? throw new TradeNotFoundException($"Trade not found with id: {id}");
            return trade;
        }

        public async Task<List<Trade>> GetAllTradesAsync()
        {
            return await dbContext.Trades
                .Include(t => t.Stock)
                .Include(b => b.Broker)
                .ToListAsync();
        }
    }
}
