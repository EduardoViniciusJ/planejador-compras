using Microsoft.AspNetCore.Http;

namespace PlanejadorCompras.API.Security;

public interface IAuthCookieService
{
    void AppendAccessToken(HttpContext context, string accessToken, DateTime expiresAtUtc);

    void DeleteAccessToken(HttpContext context);
}
