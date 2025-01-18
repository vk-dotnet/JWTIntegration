namespace JwtHelper.Contracts;

public interface ITokenService
{
    string GenerateToken(string secretKey, string issuer, string audience, int expirationMinutes);
}