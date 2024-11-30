using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace covercy_task
{
       public class ApiKeyService
    {
        private readonly ApiKeyContext _context;
        private readonly IConfiguration _configuration;

        public ApiKeyService(ApiKeyContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

    
        public string GenerateApiKey()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

    
        public string GenerateJwtToken(ApiKey apiKey)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, apiKey.UserId),
                new Claim("api_key_id", apiKey.Id.ToString())
            };

            
            apiKey.Permissions.ForEach(permission => 
                claims.Add(new Claim("permissions", permission)));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
 
}