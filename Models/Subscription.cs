using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket.Models
{
    public class Subscription
    {
        [ForeignKey(nameof(Market))]
        public int MarketId { get; set; }
        public Market Market { get; set; }

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
