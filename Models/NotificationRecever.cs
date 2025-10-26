using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket.Models
{
    public class NotificationReceiver
    {
        public int NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;

        public int ReceiverId { get; set; }
        public AppUser Receiver { get; set; } = null!;

    }

}
