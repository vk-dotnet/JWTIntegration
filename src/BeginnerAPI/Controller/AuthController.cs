using JwtHelper;
using JwtHelper.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthController(ITokenService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    [HttpPost("generate-token")]
    public IActionResult GenerateToken()
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var token = _tokenService.GenerateToken(
            jwtSettings["SecretKey"], 
            jwtSettings["Issuer"], 
            jwtSettings["Audience"], 
            int.Parse(jwtSettings["ExpirationMinutes"])
        );

        return Ok(new { Token = token });
    }
    [Authorize] // Token gerektirir
    [HttpGet("secure-endpoint")]
    public IActionResult SecureEndpoint()
    {
        return Ok(new
        {
            Message = "This is a secure endpoint. You have a valid token!",
            Timestamp = DateTime.UtcNow
        });
    }
    [AllowAnonymous] // Token gerektirmez
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