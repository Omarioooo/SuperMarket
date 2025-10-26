﻿namespace SuperMarket.Reposetories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<bool> AddSenderAsync(NotificationSender sender);
        Task<bool> AddReceiversAsync(List<NotificationReceiver> receivers);
        Task<List<Notification>> GetNotificationsByUserAsync(int userId);
    }

}