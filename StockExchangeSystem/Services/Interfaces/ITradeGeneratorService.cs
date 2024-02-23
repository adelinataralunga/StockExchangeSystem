namespace StockExchangeSystem.API.Services.Interfaces
{
    public interface ITradeGeneratorService
    {
        Task<int> GenerateTradesAsync(TimeSpan duration);
    }
}
