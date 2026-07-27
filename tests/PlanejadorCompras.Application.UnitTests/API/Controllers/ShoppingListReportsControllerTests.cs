using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlanejadorCompras.API.Controllers;
using PlanejadorCompras.Application.Common.Dtos.Reports;
using PlanejadorCompras.Application.UseCases.Interfaces;

namespace PlanejadorCompras.Application.UnitTests.API.Controllers;

public sealed class ShoppingListReportsControllerTests
{
    private readonly Mock<IExportShoppingListReportUseCase> _exportReportUseCaseMock = new();

    [Fact]
    public async Task GetPdf_ShouldReturnDownloadablePdfAndForwardCancellation()
    {
        var shoppingListId = Guid.NewGuid();
        var expectedFile = new ExportedFileDto(
            new byte[] { 0x25, 0x50, 0x44, 0x46 },
            "application/pdf",
            $"equalizacao-{shoppingListId:N}.pdf");
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        _exportReportUseCaseMock
            .Setup(useCase => useCase.ExportPdfAsync(shoppingListId, cancellationToken))
            .ReturnsAsync(expectedFile);
        var controller = CreateController();

        var result = await controller.GetPdf(shoppingListId, cancellationToken);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Same(expectedFile.Content, fileResult.FileContents);
        Assert.Equal(expectedFile.ContentType, fileResult.ContentType);
        Assert.Equal(expectedFile.FileName, fileResult.FileDownloadName);
    }

    [Fact]
    public async Task GetExcel_ShouldReturnDownloadableWorkbookAndForwardCancellation()
    {
        var shoppingListId = Guid.NewGuid();
        var expectedFile = new ExportedFileDto(
            new byte[] { 0x50, 0x4B },
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"equalizacao-{shoppingListId:N}.xlsx");
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        _exportReportUseCaseMock
            .Setup(useCase => useCase.ExportExcelAsync(shoppingListId, cancellationToken))
            .ReturnsAsync(expectedFile);
        var controller = CreateController();

        var result = await controller.GetExcel(shoppingListId, cancellationToken);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Same(expectedFile.Content, fileResult.FileContents);
        Assert.Equal(expectedFile.ContentType, fileResult.ContentType);
        Assert.Equal(expectedFile.FileName, fileResult.FileDownloadName);
    }

    [Fact]
    public void Controller_ShouldDeclareProtectedAttributeRoutesAndOpenApiResponses()
    {
        var controllerType = typeof(ShoppingListReportsController);

        Assert.NotNull(controllerType.GetCustomAttribute<AuthorizeAttribute>());
        Assert.NotNull(controllerType.GetCustomAttribute<ApiControllerAttribute>());
        Assert.Equal(
            "api/shopping-lists/{id:guid}/reports",
            controllerType.GetCustomAttribute<RouteAttribute>()?.Template);

        AssertEndpointMetadata(nameof(ShoppingListReportsController.GetPdf), "pdf");
        AssertEndpointMetadata(nameof(ShoppingListReportsController.GetExcel), "excel");
    }

    private static void AssertEndpointMetadata(string methodName, string route)
    {
        var method = typeof(ShoppingListReportsController).GetMethod(methodName);
        Assert.NotNull(method);
        Assert.Equal(route, method.GetCustomAttribute<HttpGetAttribute>()?.Template);

        var documentedStatuses = method
            .GetCustomAttributes<ProducesResponseTypeAttribute>()
            .Select(attribute => attribute.StatusCode)
            .ToHashSet();

        Assert.Contains(200, documentedStatuses);
        Assert.Contains(401, documentedStatuses);
        Assert.Contains(404, documentedStatuses);
    }

    private ShoppingListReportsController CreateController() =>
        new(_exportReportUseCaseMock.Object);
}
