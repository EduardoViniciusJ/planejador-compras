using Microsoft.AspNetCore.Http;

namespace PlanejadorCompras.API.Security;

internal sealed class AuthCookieService : IAuthCookieService
{
    public void AppendAccessToken(HttpContext context, string accessToken, DateTime expiresAtUtc)
    {
        context.Response.Cookies.Append(
            AuthenticationConstants.AccessTokenCookieName,
            accessToken,
            CreateAccessTokenCookieOptions(context.Request.IsHttps, expiresAtUtc));
    }

    public void DeleteAccessToken(HttpContext context)
    {
        context.Response.Cookies.Delete(
            AuthenticationConstants.AccessTokenCookieName,
            CreateAccessTokenCookieOptions(context.Request.IsHttps));
    }

    private static CookieOptions CreateAccessTokenCookieOptions(bool isHttps, DateTime? expiresAtUtc = null)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Path = "/",
            Expires = expiresAtUtc
        };
    }
}
