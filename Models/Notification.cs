using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Message { get; set; } = null!;

        public bool IsRead { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Type))]
        public int TypeId { get; set; }

        public NotificationType Type { get; set; }

        public NotificationSender Sender { get; set; }
        public List<NotificationReceiver> Receivers { get; set; }
    }
}
