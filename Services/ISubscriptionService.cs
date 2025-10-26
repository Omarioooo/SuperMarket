namespace SuperMarket.Services
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Subscribe a client to a market and send notification
        /// </summary>
        Task<bool> SubscribeToMarketAsync(int clientId, int marketId);

        /// <summary>
        /// Unsubscribe a client from a market
        /// </summary>
        Task<bool> UnsubscribeFromMarketAsync(int clientId, int marketId);

        /// <summary>
        /// Get all markets a client is subscribed to
        /// </summary>
        Task<List<Market>> GetClientSubscriptionsAsync(int clientId);

        /// <summary>
        /// Get all subscribers of a market
        /// </summary>
        Task<List<Client>> GetMarketSubscribersAsync(int marketId);

        /// <summary>
        /// Check if a client is subscribed to a market
        /// </summary>
        Task<bool> IsSubscribedAsync(int clientId, int marketId);

    }
}