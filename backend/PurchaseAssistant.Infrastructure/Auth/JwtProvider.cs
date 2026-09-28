using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PurchaseAssistant.Application.Interfaces;
using PurchaseAssistant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PurchaseAssistant.Infrastructure.Auth
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;

        public JwtProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateAccessToken(User user, Membership? activeMembership)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (activeMembership != null)
            {
                claims.Add(new Claim("businessId", activeMembership.BusinessId.ToString()));
                claims.Add(new Claim("role", activeMembership.Role.ToString()));
                
                if (!string.IsNullOrEmpty(activeMembership.PermissionsJson))
                {
                    try
                    {
                        var perms = System.Text.Json.JsonSerializer.Deserialize<List<string>>(activeMembership.PermissionsJson);
                        if (perms != null)
                        {
                            foreach (var p in perms)
                                claims.Add(new Claim("permissions", p));
                        }
                    }
                    catch { }
                }
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _options.Issuer,
                _options.Audience,
                claims,
                null,
                DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRandomToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
