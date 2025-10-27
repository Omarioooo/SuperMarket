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
            // Validate client exists
            var client = await _unitOfWork.Clients.GetAsync(clientId);
            if (client == null)
                throw new ArgumentException($"Client with ID {clientId} not found");

            // Validate market exists
            var market = await _unitOfWork.Markets.GetAsync(marketId);
            if (market == null)
                throw new ArgumentException($"Market with ID {marketId} not found");

            // Check if market is accepted
            if (market.Status != MarketStatus.Accepted)
                throw new InvalidOperationException("Cannot subscribe to a market that is not accepted");

            // Check if already subscribed
            if (await IsSubscribedAsync(clientId, marketId))
                throw new InvalidOperationException("Client is already subscribed to this market");

            // Create subscription
            var subscription = new Subscription
            {
                ClientId = clientId,
                MarketId = marketId,
                CreatedAt = DateTime.UtcNow
            };

            // Add to database
            _unitOfWork.Clients.GetAsync(clientId).Result.Subscriptions.Add(subscription);
            await _unitOfWork.SaveAsync();

            // Notification Service
            try
            {
                await _notificationService.SendNotificationAsync(
                    senderId: clientId,
                    receiverIds: new List<int> { marketId }, // Market owner receives
                    type: NotificationsTypeEnum.NewSubscribe
                );
            }
            catch (Exception ex)
            {
                // Notification failure shouldn't break the subscription
                Console.WriteLine($"Failed to send notification: {ex.Message}");
            }

            return true;
        }

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

        public async Task<List<Market>> GetClientSubscriptionsAsync(int clientId)
        {
            var client = await _unitOfWork.Clients.GetAsync(clientId);
            if (client == null)
                throw new ArgumentException($"Client with ID {clientId} not found");

            return client.Subscriptions
                .Select(s => s.Market)
                .ToList();
        }

        public async Task<List<Client>> GetMarketSubscribersAsync(int marketId)
        {
            var market = await _unitOfWork.Markets.GetAsync(marketId);
            if (market == null)
                throw new ArgumentException($"Market with ID {marketId} not found");

            return market.Subscriptions
                .Select(s => s.Client)
                .ToList();
        }

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