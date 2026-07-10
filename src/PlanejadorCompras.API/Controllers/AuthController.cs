using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.API.Security;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.Auth;

namespace PlanejadorCompras.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly GoogleAuthorizationCodeLoginUseCase _googleAuthorizationCodeLoginUseCase;
    private readonly ICurrentUser _currentUser;
    private readonly IAuthCookieService _authCookieService;

    public AuthController(
        GoogleAuthorizationCodeLoginUseCase googleAuthorizationCodeLoginUseCase,
        ICurrentUser currentUser,
        IAuthCookieService authCookieService)
    {
        _googleAuthorizationCodeLoginUseCase = googleAuthorizationCodeLoginUseCase;
        _currentUser = currentUser;
        _authCookieService = authCookieService;
    }

    [HttpPost("google-code")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GoogleCodeLogin(
        [FromBody] GoogleAuthorizationCodeLoginRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!IsXmlHttpRequest())
        {
            return Unauthorized(CreateProblemDetails(
                StatusCodes.Status401Unauthorized,
                "Missing required Google login request header.",
                "google_code_missing_x_requested_with"));
        }

        var result = await _googleAuthorizationCodeLoginUseCase.ExecuteAsync(request, cancellationToken);

        _authCookieService.AppendAccessToken(HttpContext, result.AccessToken, result.ExpiresAtUtc);

        return Ok();
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(CurrentUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult GetMe()
    {
        return Ok(new CurrentUserResponseDto(
            _currentUser.UserId,
            _currentUser.Email,
            _currentUser.Name));
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        _authCookieService.DeleteAccessToken(HttpContext);

        return NoContent();
    }

    private bool IsXmlHttpRequest()
    {
        return string.Equals(
            Request.Headers[AuthenticationConstants.XmlHttpRequestHeaderName],
            AuthenticationConstants.XmlHttpRequestHeaderValue,
            StringComparison.OrdinalIgnoreCase);
    }

    private ProblemDetails CreateProblemDetails(int status, string title, string errorCode)
    {
        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Instance = Request.Path
        };

        problemDetails.Extensions["errorCode"] = errorCode;

        return problemDetails;
    }
}
