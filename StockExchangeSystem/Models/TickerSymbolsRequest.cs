using StockExchangeSystem.API.Validations;
using System.ComponentModel.DataAnnotations;

namespace StockExchangeSystem.API.Models
{
    public class TickerSymbolsRequest
    {
        [Required(ErrorMessage= "Ticker symbols list cannot be empty.")]
        [ValidTickerSymbols(ErrorMessage = "Invalid ticker symbols.")]
        public List<string> TickerSymbols { get; set; } = new List<string>();
    }
}
