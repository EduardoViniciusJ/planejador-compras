using Moq;
using PlanejadorCompras.Application.Common.Dtos.Requests;
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
