using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingListSupplier;
using ShoppingListEntity = PlanejadorCompras.Domain.Entities.ShoppingList;
using ShoppingListSupplierEntity = PlanejadorCompras.Domain.Entities.ShoppingListSupplier;
using SupplierEntity = PlanejadorCompras.Domain.Entities.Supplier;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList;

public sealed class ShoppingListSupplierUseCasesTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Mock<IShoppingListSupplierRepository> _repository = new();
    private readonly Mock<IShoppingListAccessService> _shoppingListAccess = new();
    private readonly Mock<ISupplierAccessService> _supplierAccess = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact]
    public async Task Get_ShouldReturnOnlySuppliersAssignedToTheList()
    {
        var list = ShoppingListEntity.Create(_userId, "Escritório");
        var supplier = SupplierEntity.Create(_userId, "Papelaria");
        _shoppingListAccess
            .Setup(service => service.GetForCurrentUserAsync(list.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
        _repository
            .Setup(repository => repository.GetSuppliersAsync(list.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SupplierEntity> { supplier });
        var useCase = new GetShoppingListSuppliersUseCase(
            _repository.Object,
            _shoppingListAccess.Object);

        var result = await useCase.ExecuteAsync(list.Id);

        Assert.Equal(supplier.Id, Assert.Single(result).Id);
    }

    [Fact]
    public async Task Add_ShouldValidateOwnershipAndPersistNewAssignment()
    {
        var list = ShoppingListEntity.Create(_userId, "Escritório");
        var supplier = SupplierEntity.Create(_userId, "Papelaria");
        ShoppingListSupplierEntity? persisted = null;
        _shoppingListAccess
            .Setup(service => service.GetForCurrentUserAsync(list.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
        _supplierAccess
            .Setup(service => service.GetForCurrentUserAsync(supplier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);
        _repository
            .Setup(repository => repository.AddAsync(
                It.IsAny<ShoppingListSupplierEntity>(),
                It.IsAny<CancellationToken>()))
            .Callback<ShoppingListSupplierEntity, CancellationToken>((link, _) => persisted = link);
        var useCase = new AddSupplierToShoppingListUseCase(
            _repository.Object,
            _shoppingListAccess.Object,
            _supplierAccess.Object,
            _unitOfWork.Object);

        var result = await useCase.ExecuteAsync(list.Id, supplier.Id);

        Assert.NotNull(persisted);
        Assert.Equal(list.Id, persisted.ShoppingListId);
        Assert.Equal(supplier.Id, persisted.SupplierId);
        Assert.Equal(supplier.Id, result.Id);
        _unitOfWork.Verify(unit => unit.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Add_ShouldBeIdempotentWhenSupplierIsAlreadyAssigned()
    {
        var list = ShoppingListEntity.Create(_userId, "Escritório");
        var supplier = SupplierEntity.Create(_userId, "Papelaria");
        _shoppingListAccess
            .Setup(service => service.GetForCurrentUserAsync(list.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
        _supplierAccess
            .Setup(service => service.GetForCurrentUserAsync(supplier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);
        _repository
            .Setup(repository => repository.ExistsAsync(
                list.Id,
                supplier.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var useCase = new AddSupplierToShoppingListUseCase(
            _repository.Object,
            _shoppingListAccess.Object,
            _supplierAccess.Object,
            _unitOfWork.Object);

        await useCase.ExecuteAsync(list.Id, supplier.Id);

        _repository.Verify(
            repository => repository.AddAsync(
                It.IsAny<ShoppingListSupplierEntity>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWork.Verify(unit => unit.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Remove_ShouldReturnNotFoundWhenSupplierIsNotAssigned()
    {
        var list = ShoppingListEntity.Create(_userId, "Escritório");
        var supplier = SupplierEntity.Create(_userId, "Papelaria");
        _shoppingListAccess
            .Setup(service => service.GetForCurrentUserAsync(list.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
        _supplierAccess
            .Setup(service => service.GetForCurrentUserAsync(supplier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);
        var useCase = new RemoveSupplierFromShoppingListUseCase(
            _repository.Object,
            _shoppingListAccess.Object,
            _supplierAccess.Object,
            _unitOfWork.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            useCase.ExecuteAsync(list.Id, supplier.Id));

        _unitOfWork.Verify(unit => unit.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
