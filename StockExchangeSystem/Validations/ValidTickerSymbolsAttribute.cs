using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace StockExchangeSystem.API.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidTickerSymbolsAttribute : ValidationAttribute
    {
        private readonly Regex _tickerSymbolPattern = new(@"^[A-Za-z0-9]{1,5}$", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not List<string> tickerSymbols || !tickerSymbols.Any())
            {
                return new ValidationResult("Ticker symbols list cannot be empty.");
            }
            if (tickerSymbols.Any(symbol => string.IsNullOrWhiteSpace(symbol) || !_tickerSymbolPattern.IsMatch(symbol)))
            {
                return new ValidationResult("Ticker symbols list contains invalid symbols. Each symbol must be non-empty, alphanumeric, and up to 5 characters long.");
            }

            return ValidationResult.Success!;
        }
    }
}
