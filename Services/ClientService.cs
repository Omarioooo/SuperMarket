using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SuperMarket.Services
{
    public class ClientService : IClientService
    {
        private readonly IUserContextService _userContext;
        private readonly ISubscriptionService _subscriptionService;

        public ClientService(IUserContextService userContext, ISubscriptionService subscriptionService)
        {
            _userContext = userContext;
            _subscriptionService = subscriptionService;
        }

        public async Task<bool> SubscribeToMarketAsync(int marketId)
        {
            try
            {
                int clientId = _userContext.GetCurrentUserId();

                return await _subscriptionService.SubscribeToMarketAsync(clientId, marketId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UnsubscribeFromMarketAsync(int marketId)
        {
            try
            {
                int clientId = _userContext.GetCurrentUserId();
                return await _subscriptionService.UnsubscribeFromMarketAsync(clientId, marketId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Market>> GetClientSubscriptionsAsync()
        {
            try
            {
                int clientId = _userContext.GetCurrentUserId();
                return await _subscriptionService.GetClientSubscriptionsAsync(clientId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}