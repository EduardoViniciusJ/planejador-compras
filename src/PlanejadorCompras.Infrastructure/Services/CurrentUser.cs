using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class CurrentUser : ICurrentUser
{
    private static readonly Guid AnonymousUserId = Guid.Parse("8420BEEC-1C4B-4098-8827-FA0509AD4B25");

    public Guid UserId { get; }
    public string Email { get; }
    public string Name { get; }

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            // Temporary fallback for local anonymous testing.
            UserId = AnonymousUserId;
            Email = "eduardoviniciusjorge@gmail.com";
            Name = "Eduardo Vinicius";
            return;
        }

        var subClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        UserId = Guid.TryParse(subClaim, out var userId) ? userId : Guid.Empty;
        Email = user.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? user.FindFirst(ClaimTypes.Email)?.Value
            ?? string.Empty;
        Name = user.FindFirst(JwtRegisteredClaimNames.Name)?.Value
            ?? user.FindFirst(ClaimTypes.Name)?.Value
            ?? string.Empty;
    }
}
