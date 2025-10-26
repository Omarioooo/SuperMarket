namespace SuperMarket.DTOs
{
    public class NotificationMessageDto
    {
        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
