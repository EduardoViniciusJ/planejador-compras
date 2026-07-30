using PlanejadorCompras.Application.Services.Interfaces;

namespace PlanejadorCompras.Infrastructure.Services;

public sealed class PurchaseOrderCodeGenerator : IPurchaseOrderCodeGenerator
{
    public string Generate(DateTime nowUtc) =>
        $"PC-{nowUtc:yyyy}-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
}
