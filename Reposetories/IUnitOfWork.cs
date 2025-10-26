namespace SuperMarket.Reposetories
{
    public interface IUnitOfWork : IDisposable
    {
        INotificationRepository Notifications { get; }
        //  INotificationType NotificationTypes { get; }
        IClientRepository Clients { get; }
        IMarketRepository Markets { get; }
        IProductRepository Products { get; }

        Task SaveAsync();
    }
}
