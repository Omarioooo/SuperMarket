using System.ComponentModel.DataAnnotations;

namespace SuperMarket.DTOs
{
    public class SubscriptionDto
    {
        /// <summary>
        /// DTO for subscribing/unsubscribing to a market
        /// </summary>
        public class SubscribeDto
        {
            [Required]
            public int ClientId { get; set; }

            [Required]
            public int MarketId { get; set; }
        }
    }
}
