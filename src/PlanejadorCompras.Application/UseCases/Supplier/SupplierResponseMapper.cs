using PlanejadorCompras.Application.Common.Dtos.Responses;
using SupplierEntity = PlanejadorCompras.Domain.Entities.Supplier;

namespace PlanejadorCompras.Application.UseCases.Supplier;

public static class SupplierResponseMapper
{
    public static SupplierResponseDto Map(SupplierEntity supplier) =>
        new(
            supplier.Id,
            supplier.Name,
            supplier.Cnpj,
            supplier.Address is null
                ? null
                : new SupplierAddressResponseDto(
                    supplier.Address.Street,
                    supplier.Address.City,
                    supplier.Address.PostalCode),
            supplier.Contact is null
                ? null
                : new SupplierContactResponseDto(
                    supplier.Contact.Email,
                    supplier.Contact.Phone),
            supplier.CreatedAt);
}
