namespace SuperMarket.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int senderId, List<int> receiverIds, NotificationsTypeEnum type);
        Task<List<NotificationReceiver>> GetUserNotificationsAsync(int userId);

    }
}