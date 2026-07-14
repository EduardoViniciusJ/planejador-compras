using SupplierEntity = PlanejadorCompras.Domain.Entities.Supplier;

namespace PlanejadorCompras.Application.Services.Interfaces;

public interface ISupplierAccessService
{
    Task<SupplierEntity> GetForCurrentUserAsync(Guid supplierId, CancellationToken cancellationToken = default);
}
