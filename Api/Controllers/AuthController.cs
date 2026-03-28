using Api.Extensions;
using Application.DTO;
using Application.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _auth;

    public AuthController(IAuthRepository auth)
    {
        _auth = auth;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (request is null)
            return this.ApiBadRequest("Request body is required.");

        try
        {
            var result = await _auth.LoginAsync(request);
            return this.ApiOk(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.ApiUnauthorized(ex.Message);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.RefreshToken))
            return this.ApiBadRequest("Refresh token is required.");

        try
        {
            var result = await _auth.RefreshTokenAsync(request.RefreshToken.Trim());
            return this.ApiOk(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.ApiUnauthorized(ex.Message);
        }
    }
}
