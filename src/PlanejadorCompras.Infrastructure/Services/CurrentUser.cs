using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var user = GetAuthenticatedUser();
            var subClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(subClaim, out var userId) || userId == Guid.Empty)
            {
                throw new UnauthorizedException("Authenticated user id is invalid.");
            }

            return userId;
        }
    }

    public string Email
    {
        get
        {
            var user = GetAuthenticatedUser();
            return user.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                ?? user.FindFirst(ClaimTypes.Email)?.Value
                ?? string.Empty;
        }
    }

    public string Name
    {
        get
        {
            var user = GetAuthenticatedUser();
            return user.FindFirst(JwtRegisteredClaimNames.Name)?.Value
                ?? user.FindFirst(ClaimTypes.Name)?.Value
                ?? string.Empty;
        }
    }

    private ClaimsPrincipal GetAuthenticatedUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        return user;
    }
}
