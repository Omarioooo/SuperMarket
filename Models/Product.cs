using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public byte[]? Photo { get; set; }

        public double Price { get; set; }

        public List<MarketProduct> MarketProducts { get; set; }
    }
}
