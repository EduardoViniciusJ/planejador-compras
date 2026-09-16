namespace PlanejadorCompras.Application.Services.Interfaces;

public interface IFrontendUrlService
{
    string BuildEmailConfirmationUrl(string token);

    string BuildPasswordResetUrl(string token);
}
