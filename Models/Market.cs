using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket.Models
{
    public class Market
    {
        [Key]
        [ForeignKey(nameof(AppUser))]
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [Required]
        public MarketStatus Status { get; set; } = MarketStatus.Pending;

        public AppUser AppUser { get; set; }
        public List<Subscription> Subscriptions { get; set; } = new();
        public List<MarketProduct> Products { get; set; } = new();
    }
}
