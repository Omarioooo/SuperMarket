using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Services;
using System.Security.Claims;
using static SuperMarket.DTOs.SubscriptionDto;

namespace SuperMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        // ❌ NO NotificationController injection
        // ❌ NO NotificationService injection (it's used inside SubscriptionService)

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// Subscribe a client to a market
        /// The service handles BOTH subscription AND notification automatically
        /// </summary>
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeDto request)
        {
            try
            {
                // ✅ ONE line of code - Service handles everything
                var success = await _subscriptionService.SubscribeToMarketAsync(
                    request.ClientId,
                    request.MarketId
                );

                if (success)
                {
                    return Ok(new
                    {
                        message = "Subscribed successfully",
                        clientId = request.ClientId,
                        marketId = request.MarketId,
                        note = "Market owner has been notified" // ✅ Notification sent automatically
                    });
                }

                return BadRequest(new { message = "Subscription failed" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        /// <summary>
        /// Unsubscribe a client from a market
        /// </summary>
        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] SubscribeDto request)
        {
            try
            {
                var success = await _subscriptionService.UnsubscribeFromMarketAsync(
                    request.ClientId,
                    request.MarketId
                );

                if (success)
                {
                    return Ok(new
                    {
                        message = "Unsubscribed successfully",
                        clientId = request.ClientId,
                        marketId = request.MarketId
                    });
                }

                return BadRequest(new { message = "Unsubscribe failed" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all markets a client is subscribed to
        /// </summary>
        [HttpGet("client/{clientId}/markets")]
        public async Task<IActionResult> GetClientSubscriptions(int clientId)
        {
            try
            {
                var markets = await _subscriptionService.GetClientSubscriptionsAsync(clientId);

                return Ok(new
                {
                    clientId = clientId,
                    totalSubscriptions = markets.Count,
                    markets = markets.Select(m => new
                    {
                        id = m.Id,
                        name = m.Name,
                        description = m.Description,
                        status = m.Status.ToString()
                    })
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all subscribers of a market
        /// </summary>
        [HttpGet("market/{marketId}/subscribers")]
        public async Task<IActionResult> GetMarketSubscribers(int marketId)
        {
            try
            {
                var clients = await _subscriptionService.GetMarketSubscribersAsync(marketId);

                return Ok(new
                {
                    marketId = marketId,
                    totalSubscribers = clients.Count,
                    subscribers = clients.Select(c => new
                    {
                        id = c.Id,
                        firstName = c.FirstName,
                        lastName = c.LastName,
                        email = c.AppUser.Email
                    })
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        /// <summary>
        /// Check if a client is subscribed to a market
        /// </summary>
        [HttpGet("check")]
        public async Task<IActionResult> CheckSubscription([FromQuery] int clientId, [FromQuery] int marketId)
        {
            try
            {
                var isSubscribed = await _subscriptionService.IsSubscribedAsync(clientId, marketId);

                return Ok(new
                {
                    clientId = clientId,
                    marketId = marketId,
                    isSubscribed = isSubscribed
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }
    }
}