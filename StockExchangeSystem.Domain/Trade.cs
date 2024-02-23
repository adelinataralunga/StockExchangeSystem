using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StockExchangeSystem.Domain
{
    public class Trade
    {
        // Explicitly marking 'Id' as the primary key, with auto-increment
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // Foreign key for database referential integrity and query optimization.
        public int StockId { get; set; }
        // Navigation property for code readability and eager loading.
        public Stock? Stock { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; } 
        public int BrokerId { get; set; }
        public Broker? Broker { get; set; } 
        public DateTime Timestamp { get; set; }

        public bool IsValid()
        {
            return Price >= 0 && Quantity > 0 && BrokerId > 0;
        }
    }
}
