using Microsoft.EntityFrameworkCore;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Data.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly StockExchangeDbContext dbContext;

        public StockRepository(StockExchangeDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task<List<Stock>> GetAllStocksAsync()
        {
            return await dbContext.Stocks.ToListAsync();
        }

        public async Task<Stock> GetStockByTickerAsync(string tickerSymbol)
        {
            var stock = await dbContext.Stocks.FirstOrDefaultAsync(s => s.TickerSymbol == tickerSymbol) ?? throw new StockNotFoundException($"Stock not found with ticker symbol: {tickerSymbol}");
            return stock;
        }

        public async Task UpdateStockPriceAsync(Stock stock)
        {
            dbContext.Entry(stock).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task AddStockAsync(Stock stock)
        {
            if (await dbContext.Stocks.AnyAsync(s => s.TickerSymbol == stock.TickerSymbol))
            {
                throw new ArgumentException("A stock with this ticker symbol already exists");
            }
            if (await dbContext.Stocks.AnyAsync(s => s.Id == stock.Id))
            {
                throw new ArgumentException("A stock with this ID already exists");
            }

            dbContext.Stocks.Add(stock);
            await dbContext.SaveChangesAsync();
        }
    }
}
