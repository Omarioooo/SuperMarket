using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket.Models
{
    public class NotificationSender
    {
        [Key]
        public int NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;

        public int SenderId { get; set; }
        public AppUser Sender { get; set; } = null!;

    }
}

