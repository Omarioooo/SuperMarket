﻿namespace SuperMarket.Hubs
{
    public interface INotificationHub
    {
        Task SendMessage(List<int> RecieversId, NotificationMessageDto message);
        Task BroadCastMessage(NotificationMessageDto message);
    }
}