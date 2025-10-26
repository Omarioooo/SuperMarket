using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SuperMarket.Models
{
    public class Context : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<MarketProduct> MarketProducts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<NotificationSender> NotificationSenders { get; set; }
        public DbSet<NotificationReceiver> NotificationReceivers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // NotificationReceiver => composite key
            builder.Entity<NotificationReceiver>()
                .HasKey(nr => new { nr.NotificationId, nr.ReceiverId });

            builder.Entity<NotificationReceiver>()
                .HasOne(nr => nr.Notification)
                .WithMany(n => n.Receivers)
                .HasForeignKey(nr => nr.NotificationId);

            builder.Entity<NotificationReceiver>()
                .HasOne(nr => nr.Receiver)
                .WithMany(u => u.Receivers)
                .HasForeignKey(nr => nr.ReceiverId);

            // Subscription => composite key
            builder.Entity<Subscription>()
                .HasKey(s => new { s.MarketId, s.ClientId });

            builder.Entity<Subscription>()
                .HasOne(s => s.Market)
                .WithMany(m => m.Subscriptions)
                .HasForeignKey(s => s.MarketId);

            builder.Entity<Subscription>()
                .HasOne(s => s.Client)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(s => s.ClientId);

            // MarketStatus as string instead of int
            builder.Entity<Market>()
                .Property(m => m.Status)
                .HasConversion<string>();

            // Insert NotificationTypes
            builder.Entity<NotificationType>().HasData(
                new NotificationType { Id = 1, Name = "NewMarket" },
                new NotificationType { Id = 2, Name = "AdminAnswer" },
                new NotificationType { Id = 3, Name = "UserSubscribed" },
                new NotificationType { Id = 4, Name = "NewProduct" }
            );
        }
    }
}
