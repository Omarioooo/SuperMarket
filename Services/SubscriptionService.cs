namespace SuperMarket.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public SubscriptionService(IUnitOfWork unitOfWork, INotificationService notificationService,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        /// <summary>
        /// Subscribe a client to a market
        /// This handles BOTH subscription logic AND notification sending
        /// </summary>
        public async Task<bool> SubscribeToMarketAsync(int clientId, int marketId)
        {
            // 1. Validate client exists
            var client = await _unitOfWork.Clients.GetAsync(clientId);
            if (client == null)
                throw new ArgumentException($"Client with ID {clientId} not found");

            // 2. Validate market exists
            var market = await _unitOfWork.Markets.GetAsync(marketId);
            if (market == null)
                throw new ArgumentException($"Market with ID {marketId} not found");

            // 3. Check if market is accepted
            if (market.Status != MarketStatus.Accepted)
                throw new InvalidOperationException("Cannot subscribe to a market that is not accepted");

            // 4. Check if already subscribed
            if (await IsSubscribedAsync(clientId, marketId))
                throw new InvalidOperationException("Client is already subscribed to this market");

            // 5. Create subscription
            var subscription = new Subscription
            {
                ClientId = clientId,
                MarketId = marketId,
                CreatedAt = DateTime.UtcNow
            };

            // 6. Add to database (assuming you have a Subscription repository)
            // Since you don't have it in UnitOfWork, we'll add it directly
            _unitOfWork.Clients.GetAsync(clientId).Result.Subscriptions.Add(subscription);

            // Alternative: if you add ISubscriptionRepository to UnitOfWork
            // await _unitOfWork.Subscriptions.AddAsync(subscription);

            // 7. Save changes
            await _unitOfWork.SaveAsync();

            // 8. ✅ Send notification to market owner
            // This is the KEY part: Service calls another Service
            try
            {
                await _notificationService.SendNotificationAsync(
                    senderId: clientId,              // Client is the sender
                    receiverIds: new List<int> { marketId }, // Market owner receives
                    type: NotificationsTypeEnum.NewSubscribe
                );
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the subscription
                // Notification failure shouldn't break the subscription
                Console.WriteLine($"Failed to send notification: {ex.Message}");
            }

            return true;
        }

        /// <summary>
        /// Unsubscribe a client from a market
        /// </summary>
        public async Task<bool> UnsubscribeFromMarketAsync(int clientId, int marketId)
        {
            var client = await _unitOfWork.Clients.GetAsync(clientId);
            if (client == null)
                throw new ArgumentException($"Client with ID {clientId} not found");

            var subscription = client.Subscriptions
                .FirstOrDefault(s => s.MarketId == marketId && s.ClientId == clientId);

            if (subscription == null)
                throw new InvalidOperationException("Client is not subscribed to this market");

            client.Subscriptions.Remove(subscription);
            await _unitOfWork.SaveAsync();

            return true;
        }

        /// <summary>
        /// Get all markets a client is subscribed to
        /// </summary>
        public async Task<List<Market>> GetClientSubscriptionsAsync(int clientId)
        {
            var client = await _unitOfWork.Clients.GetAsync(clientId);
            if (client == null)
                throw new ArgumentException($"Client with ID {clientId} not found");

            return client.Subscriptions
                .Select(s => s.Market)
                .ToList();
        }

        /// <summary>
        /// Get all subscribers of a market
        /// </summary>
        public async Task<List<Client>> GetMarketSubscribersAsync(int marketId)
        {
            var market = await _unitOfWork.Markets.GetAsync(marketId);
            if (market == null)
                throw new ArgumentException($"Market with ID {marketId} not found");

            return market.Subscriptions
                .Select(s => s.Client)
                .ToList();
        }

        /// <summary>
        /// Check if a client is subscribed to a market
        /// </summary>
        public async Task<bool> IsSubscribedAsync(int clientId, int marketId)
        {
            var client = await _unitOfWork.Clients.GetAsync(clientId);
            if (client == null)
                return false;

            return client.Subscriptions
                .Any(s => s.MarketId == marketId);
        }
    }
}
}