using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SuperMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Client")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost("[action]/{id:int}")]
        public async Task<IActionResult> Subscribe([FromRoute] int marketId)
        {
            bool isSubscribed = await _clientService.SubscribeToMarketAsync(marketId);

            if (!isSubscribed)
                return BadRequest(new { message = "Failed to subscribe to the market" });

            return Ok(new { message = "Subscribed successfully" });
        }

        [HttpDelete("[action]/{id:int}")]
        public async Task<IActionResult> UnSubscribe([FromRoute] int marketId)
        {
            bool isUnSubscribed = await _clientService.UnsubscribeFromMarketAsync(marketId);

            if (!isUnSubscribed)
                return BadRequest(new { message = "Failed to unsubscribe to the market" });

            return Ok(new { message = "Unsubscribed successfully" });
        }
    }
}
