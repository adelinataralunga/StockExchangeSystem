using Microsoft.EntityFrameworkCore;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.Data.Repositories
{
    public class BrokerRepository : IBrokerRepository
    {
        private readonly StockExchangeDbContext dbContext;

        public BrokerRepository(StockExchangeDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task<Broker> AddBrokerAsync(Broker broker)
        {
            await dbContext.Brokers.AddAsync(broker);
            await dbContext.SaveChangesAsync();
            return broker;
        }

        public async Task<List<Broker>> GetAllBrokersAsync()
        {
            return await dbContext.Brokers.ToListAsync();
        }

        public async Task<Broker> GetBrokerByIdAsync(int id)
        {
            var broker = await dbContext.Brokers.FindAsync(id) ?? throw new BrokerNotFoundException($"Broker not found with id: {id}");
            return broker;
        }
    }
}
