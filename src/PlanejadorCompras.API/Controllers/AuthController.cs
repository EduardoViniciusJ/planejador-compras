using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.UseCases.Auth;

namespace PlanejadorCompras.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly GoogleLoginUseCase _googleLoginUseCase;

    public AuthController(GoogleLoginUseCase googleLoginUseCase)
    {
        _googleLoginUseCase = googleLoginUseCase;
    }

    [HttpPost("google")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GoogleLogin(
        [FromBody] GoogleLoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _googleLoginUseCase.ExecuteAsync(request, cancellationToken);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = result.ExpiresAtUtc
        };

        Response.Cookies.Append("access_token", result.AccessToken, cookieOptions);

        return Ok();
    }
}
