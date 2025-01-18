using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtHelper.Contracts;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    // Simulate a store for refresh tokens (in-memory for now)
    private static readonly Dictionary<string, string> RefreshTokens = new();

    public AuthController(ITokenService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    // Endpoint to generate a JWT token
    [HttpPost("generate-token")]
    public IActionResult GenerateToken()
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        // Add claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "123456"), // User ID
            new Claim(JwtRegisteredClaimNames.Email, "user@example.com"),
            new Claim("Role", "Admin"),
            new Claim("Department", "IT")
        };

        var token = _tokenService.GenerateTokenIntermediate(
            jwtSettings["SecretKey"],
            jwtSettings["Issuer"],
            jwtSettings["Audience"],
            int.Parse(jwtSettings["ExpirationMinutes"]),
            claims
        );

        // Generate a refresh token (mocked as a GUID for now)
        var refreshToken = Guid.NewGuid().ToString();
        RefreshTokens[refreshToken] = token; // Store refresh token

        return Ok(new { Token = token, RefreshToken = refreshToken });
    }

    // Endpoint to refresh a JWT token
    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (RefreshTokens.ContainsKey(request.RefreshToken))
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            // Add claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "123456"), // User ID
                new Claim(JwtRegisteredClaimNames.Email, "user@example.com"),
                new Claim("Role", "Admin"),
                new Claim("Department", "IT")
            };

            // Generate a new JWT token
            var newToken = _tokenService.GenerateTokenIntermediate(
                jwtSettings["SecretKey"],
                jwtSettings["Issuer"],
                jwtSettings["Audience"],
                int.Parse(jwtSettings["ExpirationMinutes"]),
                claims
            );

            // Replace the old refresh token
            var newRefreshToken = Guid.NewGuid().ToString();
            RefreshTokens.Remove(request.RefreshToken);
            RefreshTokens[newRefreshToken] = newToken;

            return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
        }

        return Unauthorized(new { Message = "Invalid refresh token." });
    }

    // Secure endpoint that requires a valid JWT token
    [Authorize]
    [HttpGet("secure-endpoint")]
    public IActionResult SecureEndpoint()
    {
        return Ok(new
        {
            Message = "This is a secure endpoint. You have a valid token!",
            Timestamp = DateTime.UtcNow
        });
    }

    // Non-secure endpoint accessible without a token
    [AllowAnonymous]
    [HttpGet("public-endpoint")]
    public IActionResult PublicEndpoint()
    {
        return Ok(new
        {
            Message = "This is a public endpoint. No token required!",
            Timestamp = DateTime.UtcNow
        });
    }
}

// Model for the refresh token request
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
}