namespace SuperMarket.Services
{
    public interface IClientService
    {
        Task<bool> SubscribeToMarketAsync(int marketId);
        Task<bool> UnsubscribeFromMarketAsync(int marketId);
        Task<List<Market>> GetClientSubscriptionsAsync();
    }
}