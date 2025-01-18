using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtHelper.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace JwtHelper
{
    public class TokenService : ITokenService
    {
        public string GenerateToken(string secretKey, string issuer, string audience, int expirationMinutes)
        {
            // SecretKey uzunluğunu kontrol edin
            if (secretKey.Length < 32)
                throw new ArgumentException("Secret key must be at least 32 characters long.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                expires: DateTime.Now.AddMinutes(expirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateTokenIntermediate(string secretKey, string issuer, string audience, int expirationMinutes, IEnumerable<Claim> claims)
        {
            // SecretKey uzunluğunu kontrol edin
            if (secretKey.Length < 32)
                throw new ArgumentException("Secret key must be at least 32 characters long.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims: claims, // Claims ekleniyor
                expires: DateTime.Now.AddMinutes(expirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}