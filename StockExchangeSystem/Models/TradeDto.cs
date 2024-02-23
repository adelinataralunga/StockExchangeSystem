using System.ComponentModel.DataAnnotations;

namespace StockExchangeSystem.API.Models
{
    public class TradeDto
    {
        [Required(ErrorMessage = "Ticker symbol is required.")]
        [MaxLength(10, ErrorMessage = "Ticker symbol must not exceed 10 characters.")]
        public string TickerSymbol { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Broker ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Broker ID must be a positive number.")]
        public int BrokerId { get; set; }
    }
}
