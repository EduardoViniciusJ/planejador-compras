using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.PurchaseOrder;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.PurchaseOrder;

namespace PlanejadorCompras.Application.UnitTests.UseCases.PurchaseOrder;

public sealed class DeletePurchaseOrderUseCaseTests
{
    private readonly Mock<IPurchaseOrderRepository> _repository = new();
    private readonly Mock<IPurchaseOrderAccessService> _accessService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact]
    public async Task ExecuteAsync_ShouldDeleteAndCommit_WhenOrderBelongsToCurrentUser()
    {
        var id = Guid.NewGuid();
        _accessService
            .Setup(service => service.GetForCurrentUserAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanejadorCompras.Domain.Entities.PurchaseOrder)null!);
        _repository
            .Setup(repository => repository.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var useCase = CreateUseCase();

        await useCase.ExecuteAsync(id);

        _repository.Verify(
            repository => repository.DeleteAsync(id, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWork.Verify(
            unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotDelete_WhenAccessServiceRejectsOwnership()
    {
        var id = Guid.NewGuid();
        _accessService
            .Setup(service => service.GetForCurrentUserAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException(
                "Purchase order not found.",
                "purchase_order_not_found"));
        var useCase = CreateUseCase();

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(id));

        _repository.Verify(
            repository => repository.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWork.Verify(
            unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCommit_WhenOrderDisappearsBeforeDeletion()
    {
        var id = Guid.NewGuid();
        _accessService
            .Setup(service => service.GetForCurrentUserAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanejadorCompras.Domain.Entities.PurchaseOrder)null!);
        _repository
            .Setup(repository => repository.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var useCase = CreateUseCase();

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(id));

        _unitOfWork.Verify(
            unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private DeletePurchaseOrderUseCase CreateUseCase() =>
        new(_repository.Object, _accessService.Object, _unitOfWork.Object);
}
