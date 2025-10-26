using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace SuperMarket.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager,
            RoleManager<IdentityRole<int>> roleManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto model)
        {
            var userName = new MailAddress(model.Email).User;
            var user = new AppUser
            {
                Email = model.Email,
                UserName = userName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return result;

            if (!await _roleManager.RoleExistsAsync(model.Role))
                return IdentityResult.Failed(new IdentityError { Description = "Role does not exist." });

            await _userManager.AddToRoleAsync(user, model.Role);

            if (model.Role == "Client")
            {
                var client = new Client
                {
                    Id = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };
                await _unitOfWork.Clients.AddAsync(client);
            }
            else if (model.Role == "Market")
            {
                var market = new Market
                {
                    Id = user.Id,
                    Name = model.MarketName,
                    Description = model.Description,
                    Status = MarketStatus.Pending
                };
                await _unitOfWork.Markets.AddAsync(market);
            }

            return result;
        }

        public async Task<object?> LogInAsync(LoginDto model)
        {
            AppUser? user;

            if (new EmailAddressAttribute().IsValid(model.UserNameOrEmail))
                user = await _userManager.FindByEmailAsync(model.UserNameOrEmail);
            else
                user = await _userManager.FindByNameAsync(model.UserNameOrEmail);

            if (user == null)
                return null;

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return null;

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // Create JWT Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new
            {
                message = "Login succeeded",
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };
        }
    }
}
