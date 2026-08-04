using Moq;
using PlanejadorCompras.Application.Common.Dtos.Models;
using PlanejadorCompras.Application.Common.Dtos.Reports;
using PlanejadorCompras.Application.Common.Dtos.Requests;
using PlanejadorCompras.Application.Common.Dtos.Responses;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.QuotationRequest;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Repositories;
using PlanejadorCompras.Domain.Repositories.QuotationRequest;
using QuotationRequestEntity = PlanejadorCompras.Domain.Entities.QuotationRequest;

namespace PlanejadorCompras.Application.UnitTests.UseCases.QuotationRequest;

public sealed class QuotationRequestUseCasesTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 8, 4, 15, 30, 0, TimeSpan.Zero);
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _listId = Guid.NewGuid();
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly Mock<IShoppingListDetailQuery> _detailQuery = new();
    private readonly Mock<IQuotationRequestRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    public QuotationRequestUseCasesTests()
    {
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);
        _currentUser.SetupGet(user => user.Name).Returns("Marina Compradora");
        _currentUser.SetupGet(user => user.Email).Returns("marina@empresa.com.br");
    }

    [Fact]
    public async Task Create_ShouldPersistAnImmutableSnapshotAndReturnItsCode()
    {
        var sourceItemId = Guid.NewGuid();
        _detailQuery
            .Setup(query => query.GetByIdAsync(
                _userId,
                _listId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDetail(sourceItemId));
        QuotationRequestEntity? persisted = null;
        _repository
            .Setup(repository => repository.AddAsync(
                It.IsAny<QuotationRequestEntity>(),
                It.IsAny<CancellationToken>()))
            .Callback<QuotationRequestEntity, CancellationToken>(
                (request, _) => persisted = request)
            .Returns(Task.CompletedTask);

        var result = await CreateUseCase().ExecuteAsync(
            _listId,
            new QuotationRequestPdfRequestDto(
                new DateOnly(2026, 8, 10),
                "  Rua A, 10  ",
                "  Informar frete.  "));

        Assert.NotNull(persisted);
        Assert.Equal(result.Id, persisted.Id);
        Assert.StartsWith("SC-2026-", result.Code);
        Assert.Equal("Equipamentos de TI", persisted.ShoppingListName);
        Assert.Equal("Rua A, 10", persisted.DeliveryAddress);
        Assert.Equal("Informar frete.", persisted.Instructions);
        var item = Assert.Single(persisted.Items);
        Assert.Equal(sourceItemId, item.SourceShoppingItemId);
        Assert.Equal("Mouse", item.Name);
        _unitOfWork.Verify(
            unit => unit.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ShouldRejectADeadlineBeforeTheEmissionDate()
    {
        _detailQuery
            .Setup(query => query.GetByIdAsync(
                _userId,
                _listId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDetail(Guid.NewGuid()));

        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            CreateUseCase().ExecuteAsync(
                _listId,
                new QuotationRequestPdfRequestDto(new DateOnly(2026, 8, 3))));

        Assert.Equal("quotation_request_invalid_deadline", exception.ErrorCode);
        _repository.Verify(
            repository => repository.AddAsync(
                It.IsAny<QuotationRequestEntity>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetById_ShouldHideARequestOutsideTheCurrentUserScope()
    {
        _repository
            .Setup(repository => repository.GetByIdForUserAsync(
                It.IsAny<Guid>(),
                _userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((QuotationRequestEntity?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            new GetQuotationRequestByIdUseCase(
                _repository.Object,
                _currentUser.Object)
                .ExecuteAsync(Guid.NewGuid()));

        Assert.Equal("quotation_request_not_found", exception.ErrorCode);
    }

    [Fact]
    public async Task Export_ShouldUseThePersistedSnapshot()
    {
        var request = CreateEntity();
        _repository
            .Setup(repository => repository.GetByIdForUserAsync(
                request.Id,
                _userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);
        var exporter = new Mock<IQuotationRequestPdfExporter>();
        QuotationRequestReportDataDto? exported = null;
        var expected = new ExportedFileDto([1, 2, 3], "application/pdf", "request.pdf");
        exporter
            .Setup(service => service.ExportAsync(
                It.IsAny<QuotationRequestReportDataDto>(),
                It.IsAny<CancellationToken>()))
            .Callback<QuotationRequestReportDataDto, CancellationToken>(
                (data, _) => exported = data)
            .ReturnsAsync(expected);

        var result = await new ExportSavedQuotationRequestPdfUseCase(
            _repository.Object,
            _currentUser.Object,
            exporter.Object).ExecuteAsync(request.Id);

        Assert.Same(expected, result);
        Assert.NotNull(exported);
        Assert.Equal(request.Code, exported.Code);
        Assert.Equal(DateOnly.FromDateTime(request.CreatedAtUtc), exported.IssuedOn);
        Assert.Equal("Mouse", Assert.Single(exported.Items).Name);
    }

    private CreateQuotationRequestUseCase CreateUseCase() =>
        new(
            new GetShoppingListDetailUseCase(
                _currentUser.Object,
                _detailQuery.Object),
            _repository.Object,
            _currentUser.Object,
            _unitOfWork.Object,
            new FixedTimeProvider(Now));

    private ShoppingListDetailResponseDto CreateDetail(Guid sourceItemId) =>
        new(
            _listId,
            "Equipamentos de TI",
            "Renovação do escritório",
            Now.UtcDateTime.AddDays(-2),
            1,
            0,
            0,
            [new ShoppingListDetailItemDto(
                sourceItemId,
                "Mouse",
                3,
                "un",
                Now.UtcDateTime.AddDays(-2),
                0,
                null)]);

    private QuotationRequestEntity CreateEntity() =>
        QuotationRequestEntity.Create(
            _userId,
            _listId,
            "Equipamentos de TI",
            "Renovação do escritório",
            "Marina Compradora",
            "marina@empresa.com.br",
            new DateOnly(2026, 8, 10),
            "Rua A, 10",
            "Informar frete.",
            [new QuotationRequestItemSnapshot(
                Guid.NewGuid(),
                "Mouse",
                3,
                "un")],
            Now.UtcDateTime);

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
