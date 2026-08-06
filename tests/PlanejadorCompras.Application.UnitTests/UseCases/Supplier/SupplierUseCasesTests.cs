using PlanejadorCompras.Application.Features.Suppliers.Contracts;
using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.Supplier;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.Supplier;
using SupplierEntity = PlanejadorCompras.Domain.Entities.Supplier;

namespace PlanejadorCompras.Application.UnitTests.UseCases.Supplier;

public sealed class SupplierUseCasesTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Mock<ISupplierRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly Mock<ISupplierAccessService> _accessService = new();

    public SupplierUseCasesTests()
    {
        _currentUser.Setup(user => user.UserId).Returns(_userId);
    }

    [Fact]
    public void Create_ShouldTrimNameAndAssignOwner()
    {
        var supplier = SupplierEntity.Create(_userId, "  Papelaria Central  ");

        Assert.Equal(_userId, supplier.UserId);
        Assert.Equal("Papelaria Central", supplier.Name);
        Assert.NotEqual(Guid.Empty, supplier.Id);
    }

    [Fact]
    public async Task Create_ShouldPersistSupplierForCurrentUser()
    {
        SupplierEntity? persisted = null;
        _repository
            .Setup(repository => repository.AddAsync(It.IsAny<SupplierEntity>(), It.IsAny<CancellationToken>()))
            .Callback<SupplierEntity, CancellationToken>((supplier, _) => persisted = supplier);
        var useCase = new CreateSupplierUseCase(
            _repository.Object,
            _unitOfWork.Object,
            _currentUser.Object);

        var result = await useCase.ExecuteAsync(new SupplierRequestDto("Papelaria Central"));

        Assert.NotNull(persisted);
        Assert.Equal(_userId, persisted.UserId);
        Assert.Equal(persisted.Id, result.Id);
        _unitOfWork.Verify(unit => unit.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldNormalizeAndPersistCommercialProfile()
    {
        SupplierEntity? persisted = null;
        _repository
            .Setup(repository => repository.AddAsync(It.IsAny<SupplierEntity>(), It.IsAny<CancellationToken>()))
            .Callback<SupplierEntity, CancellationToken>((supplier, _) => persisted = supplier);
        var useCase = new CreateSupplierUseCase(
            _repository.Object,
            _unitOfWork.Object,
            _currentUser.Object);

        var result = await useCase.ExecuteAsync(new SupplierRequestDto(
            "  Papelaria Central  ",
            "11.222.333/0001-81",
            new SupplierAddressRequestDto(" Rua Central, 10 ", " Curitiba ", "80000-000"),
            new SupplierContactRequestDto(" COMPRAS@CENTRAL.COM.BR ", "(41) 99999-9999")));

        Assert.NotNull(persisted);
        Assert.Equal("11222333000181", persisted.Cnpj);
        Assert.Equal("Rua Central, 10", persisted.Address?.Street);
        Assert.Equal("Curitiba", persisted.Address?.City);
        Assert.Equal("80000000", persisted.Address?.PostalCode);
        Assert.Equal("compras@central.com.br", persisted.Contact?.Email);
        Assert.Equal("41999999999", persisted.Contact?.Phone);
        Assert.Equal(persisted.Cnpj, result.Cnpj);
        Assert.Equal(persisted.Address?.Street, result.Address?.Street);
    }

    [Fact]
    public async Task Create_ShouldRejectInvalidCnpj()
    {
        var useCase = new CreateSupplierUseCase(
            _repository.Object,
            _unitOfWork.Object,
            _currentUser.Object);

        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            useCase.ExecuteAsync(new SupplierRequestDto("Papelaria Central", "11.111.111/1111-11")));

        Assert.Equal("supplier_invalid_cnpj", exception.ErrorCode);
        _unitOfWork.Verify(unit => unit.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_ShouldRejectDuplicateCnpj()
    {
        _repository
            .Setup(repository => repository.ExistsByCnpjAsync(
                _userId,
                "11222333000181",
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var useCase = new CreateSupplierUseCase(
            _repository.Object,
            _unitOfWork.Object,
            _currentUser.Object);

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            useCase.ExecuteAsync(new SupplierRequestDto(
                "Papelaria Central",
                "11.222.333/0001-81")));

        Assert.Equal("supplier_cnpj_already_exists", exception.ErrorCode);
    }

    [Fact]
    public async Task Create_ShouldRejectDuplicateName()
    {
        _repository
            .Setup(repository => repository.ExistsByNameAsync(
                _userId,
                "Papelaria Central",
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var useCase = new CreateSupplierUseCase(
            _repository.Object,
            _unitOfWork.Object,
            _currentUser.Object);

        await Assert.ThrowsAsync<ConflictException>(() =>
            useCase.ExecuteAsync(new SupplierRequestDto("Papelaria Central")));

        _unitOfWork.Verify(unit => unit.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAll_ShouldReadOnlyCurrentUserSuppliers()
    {
        var supplier = SupplierEntity.Create(_userId, "Papelaria Central");
        _repository
            .Setup(repository => repository.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SupplierEntity> { supplier });
        var useCase = new GetSuppliersUseCase(_repository.Object, _currentUser.Object);

        var result = await useCase.ExecuteAsync();

        Assert.Equal(supplier.Id, Assert.Single(result).Id);
    }

    [Fact]
    public async Task Delete_ShouldRejectSupplierWithQuotes()
    {
        var supplier = SupplierEntity.Create(_userId, "Papelaria Central");
        _accessService
            .Setup(service => service.GetForCurrentUserAsync(supplier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(supplier);
        _repository
            .Setup(repository => repository.HasQuotesAsync(supplier.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var useCase = new DeleteSupplierUseCase(
            _repository.Object,
            _unitOfWork.Object,
            _accessService.Object);

        await Assert.ThrowsAsync<ConflictException>(() => useCase.ExecuteAsync(supplier.Id));

        _repository.Verify(
            repository => repository.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
