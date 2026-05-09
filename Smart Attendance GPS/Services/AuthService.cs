using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Smart_Attendance_GPS.Helpers;
using Smart_Attendance_GPS.Models;
using Smart_Attendance_GPS.Services.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Smart_Attendance_GPS.Services
{
    public class AuthService: IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<string> GenerateTokenForUser(ApplicationUser user)
        {
            return await GenerateJwtToken(user);
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            // Get user roles (optional, if you need role-based claims)
            var roles = await _userManager.GetRolesAsync(user);

            // Define claims for the JWT token
            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id),
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(ClaimTypes.Email, user.Email),
            };
            foreach (var role in roles)
            {
                claims = claims.Concat(new[]
                {
                new Claim(ClaimTypes.Role, role)
            }).ToArray();
            }

            // Create the JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddDays(_jwtSettings.DurationInDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<bool> ValidateUserCredentials(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }
    }
}
