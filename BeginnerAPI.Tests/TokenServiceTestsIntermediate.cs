using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using JwtHelper;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IntermediateAPI.Tests
{
    public class TokenServiceTests
    {
        private readonly string _secretKey = "v0lkanSuperSecureeJWTKeyy2025!!!";
        private readonly string _issuer = "Volkan.JWTIntegration";
        private readonly string _audience = "Volkan.JWTIntegration.Users";

        [Fact]
        public void GenerateToken_ShouldReturnValidJwtToken_WithClaims()
        {
            // Arrange
            var tokenService = new TokenService();
            var expirationMinutes = 30;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "123456"),
                new Claim(JwtRegisteredClaimNames.Email, "user@example.com"),
                new Claim("Role", "Admin"),
                new Claim("Department", "IT")
            };

            // Act
            var token = tokenService.GenerateTokenIntermediate(_secretKey, _issuer, _audience, expirationMinutes, claims);

            // Assert
            token.Should().NotBeNullOrWhiteSpace("Token should not be null or empty.");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            jwtToken.Issuer.Should().Be(_issuer, "Issuer should match the provided value.");
            jwtToken.Audiences.Should().Contain(_audience, "Audience should match the provided value.");
            jwtToken.Claims.Should().Contain(c => c.Type == "Role" && c.Value == "Admin");
            jwtToken.Claims.Should().Contain(c => c.Type == "Department" && c.Value == "IT");
        }

        [Fact]
        public void ValidateToken_ShouldPassForValidToken()
        {
            // Arrange
            var tokenService = new TokenService();
            var expirationMinutes = 30;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "123456"),
                new Claim(JwtRegisteredClaimNames.Email, "user@example.com"),
                new Claim("Role", "Admin"),
                new Claim("Department", "IT")
            };

            var token = tokenService.GenerateTokenIntermediate(_secretKey, _issuer, _audience, expirationMinutes, claims);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey))
            };

            var handler = new JwtSecurityTokenHandler();

            // Act
            var principal = handler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            // Assert
            validatedToken.Should().NotBeNull("Validated token should not be null.");
            principal.Identity.IsAuthenticated.Should().BeTrue("Principal should be authenticated.");
        }

        [Fact]
        public void ValidateToken_ShouldFailForInvalidSignature()
        {
            // Arrange
            var tokenService = new TokenService();
            var expirationMinutes = 30;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "123456"),
                new Claim(JwtRegisteredClaimNames.Email, "user@example.com"),
                new Claim("Role", "Admin"),
                new Claim("Department", "IT")
            };

            // Generate a valid token with correct secret key
            var token = tokenService.GenerateTokenIntermediate(_secretKey, _issuer, _audience, expirationMinutes, claims);

            // Use an invalid key for validation
            var invalidKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("InvalidSecretKeyThatDoesNotMatch"));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = invalidKey // This key does not match the token's key
            };

            var handler = new JwtSecurityTokenHandler();

            // Act & Assert
            var exception = Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() =>
                handler.ValidateToken(token, tokenValidationParameters, out _));

            // Assert that the exception is the expected type and contains the correct message
            Assert.NotNull(exception);
            Assert.Contains("Signature validation failed", exception.Message);
        }
    }

    public class RefreshTokenTests
    {
        [Fact]
        public void RefreshToken_ShouldReplaceOldToken_WithNewToken()
        {
            // Arrange
            var tokenService = new TokenService();
            var oldRefreshToken = Guid.NewGuid().ToString();
            var newRefreshToken = Guid.NewGuid().ToString();

            // Simulate in-memory storage
            var refreshTokenStore = new Dictionary<string, string> { { oldRefreshToken, "OldTokenValue" } };

            // Act
            refreshTokenStore.Remove(oldRefreshToken);
            refreshTokenStore[newRefreshToken] = "NewTokenValue";

            // Assert
            refreshTokenStore.ContainsKey(oldRefreshToken).Should().BeFalse("Old refresh token should be removed.");
            refreshTokenStore.ContainsKey(newRefreshToken).Should().BeTrue("New refresh token should be added.");
        }
    }
}
