namespace SuperMarket.Reposetories
{
    public interface IUnitOfWork : IDisposable
    {
        INotificationRepository Notifications { get; }
        //  INotificationType NotificationTypes { get; }
        IClientRepository Clients { get; }
        IMarketRepository Markets { get; }
        IProductRepository Products { get; }
        INotificationTypeRepository NotificationTypes { get; }

        DbSet<MarketProduct> MarketProducts { get; }
        DbSet<Subscription> Subscriptions { get; }


        Task SaveAsync();
    }
}
