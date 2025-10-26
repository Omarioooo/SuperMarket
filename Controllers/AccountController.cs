using System.ComponentModel.DataAnnotations;

namespace SuperMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork,
            IAccountService accountService, IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _configuration = configuration;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterDto registerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(registerRequest);

            var registerResult = await _accountService.RegisterAsync(registerRequest);

            if (!registerResult.Succeeded)
                return BadRequest(registerResult.Errors);

            await _unitOfWork.SaveAsync();
            return Ok("registered Successfully");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginDto loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var loginResult = await _accountService.LogInAsync(loginRequest);

                if (loginResult == null)
                {
                    // Differentiate between user not found and incorrect password
                    var userExists = new EmailAddressAttribute().IsValid(loginRequest.UserNameOrEmail)
                        ? await _accountService.FindByEmailAsync(loginRequest.UserNameOrEmail)
                        : await _accountService.FindByNameAsync(loginRequest.UserNameOrEmail);

                    return Unauthorized(new
                    {
                        message = userExists == null ? "User not found" : "Invalid password"
                    });
                }

                return Ok(loginResult);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = $"Configuration error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login. Please try again later." });
            }
        }
    }
}
