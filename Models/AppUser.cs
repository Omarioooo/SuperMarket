namespace SuperMarket.Models
{
    public class AppUser : IdentityUser<int>
    {
        public byte[]? Photo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<NotificationReceiver> Receivers { get; set; } = new();

    }
}