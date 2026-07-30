using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class SavedEqualizationCodeGenerator : ISavedEqualizationCodeGenerator
{
    public string Generate(DateTime nowUtc) =>
        $"EQ-{nowUtc:yyyy}-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
}
