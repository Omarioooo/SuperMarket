using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Models
{
    public class NotificationType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
