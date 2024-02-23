using System.ComponentModel.DataAnnotations;

namespace StockExchangeSystem.API.Models
{
    public class StockDto
    {
        [Required(ErrorMessage = "Ticker symbol is required.")]
        [MaxLength(10, ErrorMessage = "Ticker symbol must not exceed 10 characters.")]
        public string TickerSymbol { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Current price must be greater than 0.")]
        public decimal CurrentPrice { get; set; }
    }
}
