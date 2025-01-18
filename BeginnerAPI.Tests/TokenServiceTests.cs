using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FluentAssertions;
using JwtHelper;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace BeginnerAPI.Tests
{
    public class TokenServiceTests
    {
        private readonly string _secretKey = "v0lkanSuperSecureeJWTKeyy2025!!!";
        private readonly string _issuer = "Volkan.JWTIntegration";
        private readonly string _audience = "Volkan.JWTIntegration.Users";


        [Fact]
        public void GenerateToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var tokenService = new TokenService();
            var expirationMinutes = 30;

            // Act
            var token = tokenService.GenerateToken(_secretKey, _issuer, _audience, expirationMinutes);

            // Assert
            token.Should().NotBeNullOrWhiteSpace("Token should not be null or empty.");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            jwtToken.Issuer.Should().Be(_issuer, "Issuer should match the provided value.");
            jwtToken.Audiences.Should().Contain(_audience, "Audience should match the provided value.");
        }

        [Fact]
        public void ValidateToken_ShouldPassForValidToken()
        {
            // Arrange
            var tokenService = new TokenService();
            var expirationMinutes = 30;

            var token = tokenService.GenerateToken(_secretKey, _issuer, _audience, expirationMinutes);

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

            var token = tokenService.GenerateToken(_secretKey, _issuer, _audience, expirationMinutes);

            var invalidKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("InvalidSecretKeyThatDoesNotMatch"));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = invalidKey
            };

            var handler = new JwtSecurityTokenHandler();

            // Act & Assert
            Assert.Throws<Microsoft.IdentityModel.Tokens.SecurityTokenSignatureKeyNotFoundException>(() =>
                handler.ValidateToken(token, tokenValidationParameters, out _));
        }
    }
}