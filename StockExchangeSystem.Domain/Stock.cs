using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace StockExchangeSystem.Domain
{
    public class Stock
    {
        // Explicitly marking 'Id' as the primary key, with auto-increment
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TickerSymbol { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(TickerSymbol)
                   && TickerSymbol.Length >= 2 && TickerSymbol.Length <= 6
                   && Regex.IsMatch(TickerSymbol, @"^[A-Z0-9]+$")
                   && CurrentPrice >= 0;
        }
    }
}
