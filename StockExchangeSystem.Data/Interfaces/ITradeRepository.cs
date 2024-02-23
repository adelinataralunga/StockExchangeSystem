using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Data.Interfaces
{
    public interface ITradeRepository
    {
        Task AddTradeAsync(Trade trade);
        Task<Trade> GetTradeByIdAsync(int id);
        Task<List<Trade>> GetAllTradesAsync();
    }
}
