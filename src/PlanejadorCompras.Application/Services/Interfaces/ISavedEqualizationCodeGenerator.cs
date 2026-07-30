namespace PlanejadorCompras.Application.Services.Interfaces;

public interface ISavedEqualizationCodeGenerator
{
    string Generate(DateTime nowUtc);
}
