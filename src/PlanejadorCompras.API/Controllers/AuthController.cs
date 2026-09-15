using PlanejadorCompras.Application.Features.Authentication.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.API.Security;
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
        if (!string.Equals(
            Request.Headers[AuthenticationConstants.XmlHttpRequestHeaderName],
            AuthenticationConstants.XmlHttpRequestHeaderValue,
            StringComparison.OrdinalIgnoreCase))
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Missing required Google login request header.",
                Instance = Request.Path
            };

            problemDetails.Extensions["errorCode"] = "google_code_missing_x_requested_with";

            return Unauthorized(problemDetails);
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

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request,
        [FromServices] RegisterUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(new { message = "Usuário registrado com sucesso. Por favor, verifique seu e-mail para confirmar a conta." });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        [FromServices] LoginUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        _authCookieService.AppendAccessToken(HttpContext, result.AccessToken, result.ExpiresAtUtc);
        return Ok(new { message = "Login realizado com sucesso." });
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(
        [FromBody] ConfirmEmailRequestDto request,
        [FromServices] ConfirmEmailUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(new { message = "E-mail confirmado com sucesso." });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequestDto request,
        [FromServices] ForgotPasswordUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(new { message = "Se o e-mail existir em nossa base, você receberá um link para redefinir sua senha." });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequestDto request,
        [FromServices] ResetPasswordUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(new { message = "Senha redefinida com sucesso." });
    }
}
