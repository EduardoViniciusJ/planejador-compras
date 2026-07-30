using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlanejadorCompras.Application.Services;
using PlanejadorCompras.Application.UseCases.Auth;
using PlanejadorCompras.Application.UseCases.ItemQuote.Create;
using PlanejadorCompras.Application.UseCases.ShoppingItem.Create;
using PlanejadorCompras.Application.UseCases.ShoppingList.Create;
using PlanejadorCompras.Application.UseCases.ItemQuote;
using PlanejadorCompras.Application.UseCases.ShoppingItem;
using PlanejadorCompras.Application.UseCases.ShoppingList;
using PlanejadorCompras.Application.UseCases.Interfaces;
using PlanejadorCompras.Application.UseCases.Supplier;
using PlanejadorCompras.Application.UseCases.PurchaseOrder;
using PlanejadorCompras.Application.UseCases.Equalization;

namespace PlanejadorCompras.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<ShoppingListComparisonCalculator>();
        services.AddScoped<PurchaseOrderDraftService>();
        AddUseCases(services);

        return services;
    }

    private static void AddUseCases(IServiceCollection services)
    {
        // Auth
        services.AddScoped<GoogleLoginUseCase>();
        services.AddScoped<GoogleAuthorizationCodeLoginUseCase>();
        // ShoppingList
        services.AddScoped<CreateShoppingListUseCase>();
        services.AddScoped<GetShoppingListByIdUseCase>();
        services.AddScoped<GetShoppingListDetailUseCase>();
        services.AddScoped<GetShoppingListsByUserIdUseCase>();
        services.AddScoped<UpdateShoppingListUseCase>();
        services.AddScoped<DeleteShoppingListUseCase>();
        services.AddScoped<ICalculateBestSupplierBudgetUseCase, CalculateBestSupplierBudgetUseCase>();
        services.AddScoped<IGetShoppingListEqualizationUseCase, GetShoppingListEqualizationUseCase>();
        services.AddScoped<GetShoppingListSuppliersUseCase>();
        services.AddScoped<AddSupplierToShoppingListUseCase>();
        services.AddScoped<RemoveSupplierFromShoppingListUseCase>();
        services.AddScoped<IGetShoppingListReportDataUseCase, GetShoppingListReportDataUseCase>();
        services.AddScoped<IExportShoppingListReportUseCase, ExportShoppingListReportUseCase>();

        // ShoppingItem
        services.AddScoped<CreateShoppingItemUseCase>();
        services.AddScoped<GetShoppingItemByIdUseCase>();
        services.AddScoped<GetShoppingItemsByShoppingListIdUseCase>();
        services.AddScoped<UpdateShoppingItemUseCase>();
        services.AddScoped<DeleteShoppingItemUseCase>();

        // ItemQuote
        services.AddScoped<CreateItemQuoteUseCase>();
        services.AddScoped<GetItemQuoteByIdUseCase>();
        services.AddScoped<GetItemQuotesByShoppingItemIdUseCase>();
        services.AddScoped<GetUserItemQuotesUseCase>();
        services.AddScoped<UpdateItemQuoteUseCase>();
        services.AddScoped<DeleteItemQuoteUseCase>();

        // Supplier
        services.AddScoped<CreateSupplierUseCase>();
        services.AddScoped<GetSupplierByIdUseCase>();
        services.AddScoped<GetSuppliersUseCase>();
        services.AddScoped<UpdateSupplierUseCase>();
        services.AddScoped<DeleteSupplierUseCase>();

        // PurchaseOrder
        services.AddScoped<GetPurchaseOrderDraftUseCase>();
        services.AddScoped<CreatePurchaseOrderUseCase>();
        services.AddScoped<GetPurchaseOrdersUseCase>();
        services.AddScoped<GetPurchaseOrderByIdUseCase>();
        services.AddScoped<UpdatePurchaseOrderStatusUseCase>();
        services.AddScoped<ExportPurchaseOrderPdfUseCase>();

        // Equalization history
        services.AddScoped<CreateSavedEqualizationUseCase>();
        services.AddScoped<GetSavedEqualizationsUseCase>();
        services.AddScoped<GetSavedEqualizationByIdUseCase>();
    }
}
