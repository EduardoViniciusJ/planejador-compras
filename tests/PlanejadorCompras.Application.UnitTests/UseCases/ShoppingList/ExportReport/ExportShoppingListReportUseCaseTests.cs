using PlanejadorCompras.Application.Features.Reports.Contracts;
using Moq;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Application.UseCases.Interfaces;
using PlanejadorCompras.Application.UseCases.ShoppingList;

namespace PlanejadorCompras.Application.UnitTests.UseCases.ShoppingList.ExportReport;

public sealed class ExportShoppingListReportUseCaseTests
{
    private readonly Mock<IGetShoppingListReportDataUseCase> _getReportDataUseCaseMock = new();
    private readonly Mock<IShoppingListPdfExporter> _pdfExporterMock = new();
    private readonly Mock<IShoppingListExcelExporter> _excelExporterMock = new();

    [Fact]
    public async Task ExportPdfAsync_ShouldLoadDataAndExportPdf_WithSameCancellationToken()
    {
        var shoppingListId = Guid.NewGuid();
        var reportData = CreateReportData(shoppingListId);
        var expectedFile = new ExportedFileDto(
            new byte[] { 0x25, 0x50, 0x44, 0x46 },
            "application/pdf",
            $"equalizacao-{shoppingListId:N}.pdf");
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        SetupReportData(shoppingListId, reportData, cancellationToken);
        _pdfExporterMock
            .Setup(exporter => exporter.ExportAsync(reportData, cancellationToken))
            .ReturnsAsync(expectedFile);
        var useCase = CreateUseCase();

        var result = await useCase.ExportPdfAsync(shoppingListId, cancellationToken);

        Assert.Same(expectedFile, result);
        _excelExporterMock.Verify(
            exporter => exporter.ExportAsync(
                It.IsAny<ShoppingListReportDataDto>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExportExcelAsync_ShouldLoadDataAndExportExcel_WithSameCancellationToken()
    {
        var shoppingListId = Guid.NewGuid();
        var reportData = CreateReportData(shoppingListId);
        var expectedFile = new ExportedFileDto(
            new byte[] { 0x50, 0x4B },
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"equalizacao-{shoppingListId:N}.xlsx");
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        SetupReportData(shoppingListId, reportData, cancellationToken);
        _excelExporterMock
            .Setup(exporter => exporter.ExportAsync(reportData, cancellationToken))
            .ReturnsAsync(expectedFile);
        var useCase = CreateUseCase();

        var result = await useCase.ExportExcelAsync(shoppingListId, cancellationToken);

        Assert.Same(expectedFile, result);
        _pdfExporterMock.Verify(
            exporter => exporter.ExportAsync(
                It.IsAny<ShoppingListReportDataDto>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private ExportShoppingListReportUseCase CreateUseCase() =>
        new(
            _getReportDataUseCaseMock.Object,
            _pdfExporterMock.Object,
            _excelExporterMock.Object);

    private void SetupReportData(
        Guid shoppingListId,
        ShoppingListReportDataDto reportData,
        CancellationToken cancellationToken)
    {
        _getReportDataUseCaseMock
            .Setup(useCase => useCase.ExecuteAsync(shoppingListId, cancellationToken))
            .ReturnsAsync(reportData);
    }

    private static ShoppingListReportDataDto CreateReportData(Guid shoppingListId) =>
        new(
            shoppingListId,
            "Lista de teste",
            null,
            new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc),
            new DateTimeOffset(2026, 7, 27, 12, 0, 0, TimeSpan.Zero),
            new ShoppingListReportSummaryDto(
                0,
                0,
                0,
                0,
                0,
                0m,
                0m,
                false,
                null,
                null,
                null,
                null),
            Array.Empty<ShoppingListReportSupplierDto>(),
            Array.Empty<ShoppingListReportItemDto>(),
            Array.Empty<ShoppingListReportPendingItemDto>());
}
