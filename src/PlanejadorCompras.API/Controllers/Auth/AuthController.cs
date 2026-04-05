using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.UseCases.Auth;

namespace PlanejadorCompras.API.Controllers.Auth;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController : ControllerBase
{
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin(
        [FromBody] GoogleLoginRequestDto request,
        [FromServices] GoogleLoginUseCase useCase,
        CancellationToken cancellationToken)
    {
        var response = await useCase.ExecuteAsync(request, cancellationToken);
        return Ok(response);
    }
}
