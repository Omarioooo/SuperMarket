namespace SuperMarket.Hubs
{
    internal interface INotificationHub
    {
        Task SendMessage(int userId, NotificationMessageDto message);
        Task SendMessage(List<int> userIds, NotificationMessageDto message);
        Task BroadCastMessage(NotificationMessageDto message);
    }
}