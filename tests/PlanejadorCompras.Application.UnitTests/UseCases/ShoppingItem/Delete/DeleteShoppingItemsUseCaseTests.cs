using Moq;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.ShoppingItem;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.ShoppingItem;
using Item = PlanejadorCompras.Domain.Entities.ShoppingItem;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingItem.Delete;

public sealed class DeleteShoppingItemsUseCaseTests
{
    private readonly Mock<IShoppingItemRepository> repository = new();
    private readonly Mock<IUnitOfWork> unit = new();
    private readonly Mock<IShoppingListAccessService> access = new();
    private readonly Guid listId = Guid.NewGuid();
    private DeleteShoppingItemsUseCase UseCase() => new(repository.Object, unit.Object, access.Object);

    [Fact]
    public async Task ValidatesAllItemsBeforeDeletingAndCommitsOnce()
    {
        var item = Item.Create(listId, "Cimento", 20, "saco");
        repository.Setup(repo => repo.GetByIdAsync(item.Id, It.IsAny<CancellationToken>())).ReturnsAsync(item);
        repository.Setup(repo => repo.DeleteAsync(item.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        await UseCase().ExecuteAsync(listId, [item.Id, item.Id]);
        access.Verify(service => service.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(repo => repo.DeleteAsync(item.Id, It.IsAny<CancellationToken>()), Times.Once);
        unit.Verify(work => work.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MissingOrForeignItemPreventsEntireDeletion(bool foreign)
    {
        var own = Item.Create(listId, "Cimento", 20, "saco");
        var other = Item.Create(Guid.NewGuid(), "Areia", 1, "m3");
        repository.Setup(repo => repo.GetByIdAsync(own.Id, It.IsAny<CancellationToken>())).ReturnsAsync(own);
        repository.Setup(repo => repo.GetByIdAsync(other.Id, It.IsAny<CancellationToken>())).ReturnsAsync(foreign ? other : null);
        await Assert.ThrowsAsync<NotFoundException>(() => UseCase().ExecuteAsync(listId, [own.Id, other.Id]));
        repository.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        unit.Verify(work => work.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeniedAccessDoesNotDeleteAnything()
    {
        access.Setup(service => service.GetForCurrentUserAsync(listId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Not found", "shopping_list_not_found"));
        await Assert.ThrowsAsync<NotFoundException>(() => UseCase().ExecuteAsync(listId, [Guid.NewGuid()]));
        repository.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
