using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Data.Interfaces
{
    public interface IBrokerRepository
    {
        Task<List<Broker>> GetAllBrokersAsync();
        Task<Broker> AddBrokerAsync(Broker broker);
        Task<Broker> GetBrokerByIdAsync(int id);
    }
}
