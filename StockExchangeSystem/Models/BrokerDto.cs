using System.ComponentModel.DataAnnotations;

namespace StockExchangeSystem.API.Models
{
    public class BrokerDto
    {
        [Required(ErrorMessage = "Broker name is required.")]
        [MaxLength(100, ErrorMessage = "Broker name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}
