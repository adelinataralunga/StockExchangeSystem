using StockExchangeSystem.API.Services.Interfaces;
using StockExchangeSystem.Data.Interfaces;
using StockExchangeSystem.Domain;

namespace StockExchangeSystem.API.Services
{
    public class TradeGeneratorService : ITradeGeneratorService
    {
        private readonly IStockRepository stockRepository;
        private readonly ITradeRepository tradeRepository;
        private readonly Random random = new();
        private readonly List<string> tickers = new() { "XOM", "GE", "GOOGL", "TSLA" };

        public TradeGeneratorService(IStockRepository _stockRepository, ITradeRepository _tradeRepository)
        {
            stockRepository = _stockRepository;
            tradeRepository = _tradeRepository;
        }

        public async Task<int> GenerateTradesAsync(TimeSpan duration)
        {
            var startTime = DateTime.UtcNow;
            var endTime = startTime + duration;
            int tradesGenerated = 0;

            while (DateTime.UtcNow < endTime)
            {
                string ticker = GetRandomTicker();
               
                var stock = await stockRepository.GetStockByTickerAsync(ticker);
                    
                var trade = new Trade
                {
                    StockId = stock.Id,
                    Price = GetRandomPrice(stock.CurrentPrice),
                    Quantity = GetRandomQuantity(),
                    BrokerId = GetRandomBrokerId(),
                    Timestamp = DateTime.UtcNow
                };

                await tradeRepository.AddTradeAsync(trade);
                tradesGenerated++;

                stock.CurrentPrice = trade.Price;
                await stockRepository.UpdateStockPriceAsync(stock);

                Console.WriteLine($"Trade created for stock: {ticker}");
                
                // Throttle the loop to prevent excessive resource consumption.
                await Task.Delay(TimeSpan.FromSeconds(1)); 
            }

            return (tradesGenerated);
        }

        private string GetRandomTicker()
        {
            int index = random.Next(tickers.Count);
            return tickers[index];
        }
        private decimal GetRandomPrice(decimal basePrice)
        {
            // Add or subtract up to 5% from the current price
            double variation = random.NextDouble() * 0.10 - 0.05;
            decimal decimalVariation = (decimal)(1 + variation);
            return basePrice * decimalVariation;
        }

        // Get quantities between 1 and 10
        private int GetRandomQuantity()
        {
            return random.Next(1, 10);
        }

        // Get one of the two existing brokers
        private int GetRandomBrokerId()
        {
            return random.Next(1, 3);
        }
    }
}
