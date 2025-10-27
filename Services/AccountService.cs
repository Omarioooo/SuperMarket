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
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager,
            RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto model)
        {
            if (model == null)
                return IdentityResult.Failed(new IdentityError { Description = "Invalid registration data." });

            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return IdentityResult.Failed(new IdentityError { Description = "Email and password are required." });

            var userName = new MailAddress(model.Email).User;

            using MemoryStream stream = new MemoryStream();
            await model.Photo.CopyToAsync(stream);

            var user = new AppUser
            {
                Email = model.Email.ToLowerInvariant(),
                UserName = userName,
                Photo = stream.ToArray(),
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
            if (model == null || string.IsNullOrWhiteSpace(model.UserNameOrEmail) || string.IsNullOrWhiteSpace(model.Password))
                return null;

            AppUser? user;
            string userNameOrEmail = model.UserNameOrEmail.Trim();

            if (new EmailAddressAttribute().IsValid(userNameOrEmail))
            {
                // Normalize email to lowercase to match database
                user = await _userManager.FindByEmailAsync(userNameOrEmail.ToLowerInvariant());
            }
            else
            {
                user = await _userManager.FindByNameAsync(userNameOrEmail);
            }

            if (user == null)
                return null;

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return null;

            // Rest of the method remains unchanged
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey) || Encoding.UTF8.GetBytes(secretKey).Length < 32)
                throw new InvalidOperationException("JWT SecretKey is missing or too short. It must be at least 32 bytes.");

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                throw new InvalidOperationException("JWT Issuer or Audience is missing.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                issuer: issuer,
                audience: audience,
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

        public async Task<AppUser> FindByEmailAsync(string mail)
        {
            return await _userManager.FindByEmailAsync(mail);
        }

        public async Task<AppUser> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
    }
}
