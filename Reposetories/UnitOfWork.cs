using SuperClient.Reposetories;

namespace SuperMarket.Reposetories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;
        public INotificationRepository Notifications { get; private set; }

        public IClientRepository Clients { get; private set; }

        public IMarketRepository Markets { get; private set; }

        public IProductRepository Products { get; private set; }
        public INotificationTypeRepository NotificationTypes { get; private set; }

        // Computed Property
        public DbSet<MarketProduct> MarketProducts => _context.MarketProducts;
        public DbSet<Subscription> Subscriptions => _context.Subscriptions;

        public UnitOfWork(Context context)
        {
            _context = context;
            Notifications = new NotificationRepository(_context);
            Clients = new ClientRepository(_context);
            Markets = new MarketRepository(_context);
            Products = new ProductRepository(_context);
            NotificationTypes = new NotificationTypeRepository(_context);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();

    }
}
