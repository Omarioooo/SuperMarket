namespace SuperMarket.Services
{
    using System.Security.Claims;

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentUserId()
        {
            // Current User
            var user = _httpContextAccessor.HttpContext?.User;


            if (user == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated");

            // Get The Id from Claim
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new Exception("User ID not found in token");

            return int.Parse(userIdClaim.Value);
        }
    }

}
