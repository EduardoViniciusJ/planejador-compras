using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Features.ShoppingItems.Contracts;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using Item = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.Create;

public sealed class CreateShoppingItemsUseCaseTests
{
    private readonly Mock<IShoppingItemRepository> repository = new();
    private readonly Mock<IUnitOfWork> unit = new();
    private readonly Mock<IShoppingListAccessService> access = new();
    private readonly Guid listId = Guid.NewGuid();
    private CreateShoppingItemsUseCase UseCase() => new(repository.Object, unit.Object, access.Object);
    private ShoppingItemRequestDto Request(decimal quantity = 1) => new(listId, "Cimento", quantity, "saco");

    [Fact]
    public async Task SavesAllItemsInOneCommit()
    {
        await UseCase().ExecuteAsync(listId, [Request(), Request(20)]);
        access.Verify(service => service.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(repo => repo.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        unit.Verify(work => work.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvalidLaterItemDoesNotPersistEarlierItems()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => UseCase().ExecuteAsync(listId, [Request(), Request(0)]));
        repository.Verify(repo => repo.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Never);
        unit.Verify(work => work.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RejectsInaccessibleList()
    {
        access.Setup(service => service.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Not found", "shopping_list_not_found"));
        await Assert.ThrowsAsync<NotFoundException>(() => UseCase().ExecuteAsync(listId, [Request()]));
        repository.Verify(repo => repo.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RejectsEmptyOversizedAndMixedListBatches()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => UseCase().ExecuteAsync(listId, []));
        await Assert.ThrowsAsync<BadRequestException>(() => UseCase().ExecuteAsync(listId, Enumerable.Repeat(Request(), 201).ToArray()));
        await Assert.ThrowsAsync<BadRequestException>(() => UseCase().ExecuteAsync(Guid.NewGuid(), [Request()]));
        unit.Verify(work => work.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
