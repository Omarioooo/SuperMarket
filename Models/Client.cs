using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket.Models
{
    public class Client
    {
        [Key]
        [ForeignKey(nameof(AppUser))]
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }

        public AppUser AppUser { get; set; } = null!;
        public List<Subscription> Subscriptions { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
    }
}
