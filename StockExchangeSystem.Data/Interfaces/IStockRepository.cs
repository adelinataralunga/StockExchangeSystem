using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Data.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllStocksAsync();
        Task<Stock> GetStockByTickerAsync(string tickerSymbol);
        Task UpdateStockPriceAsync(Stock stock);
        Task AddStockAsync(Stock stock);
    }
}
