namespace SuperMarket.Reposetories
{
    public interface INotificationTypeRepository
    {
        Task<NotificationType?> GetNotificationTypeByIdAsync(int id);
        Task<NotificationType?> GetNotificationTypeByNameAsync(string name);
    }

}