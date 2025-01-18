using System.Security.Claims;

namespace JwtHelper.Contracts;

public interface ITokenService
{
    string GenerateToken(string secretKey, string issuer, string audience, int expirationMinutes);
    string GenerateTokenIntermediate(string secretKey, string issuer, string audience, int expirationMinutes, IEnumerable<Claim> claims);
}