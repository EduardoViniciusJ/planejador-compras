using Microsoft.Extensions.DependencyInjection;
using PlanejadorCompras.Application.UseCases.Auth;
using PlanejadorCompras.Application.UseCases.ItemQuote.Create;
using PlanejadorCompras.Application.UseCases.ShoppingItem.Create;
using PlanejadorCompras.Application.UseCases.ShoppingList.Create;
using PlanejadorCompras.Application.UseCases.ItemQuote;
using PlanejadorCompras.Application.UseCases.ShoppingItem;
using PlanejadorCompras.Application.UseCases.ShoppingList;

namespace PlanejadorCompras.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        AddUseCases(services);

        return services;
    }

    private static void AddUseCases(IServiceCollection services)
    {
        // Auth
        services.AddScoped<GoogleLoginUseCase>();

        // ShoppingList
        services.AddScoped<CreateShoppingListUseCase>();
        services.AddScoped<GetShoppingListByIdUseCase>();
        services.AddScoped<GetShoppingListsByUserIdUseCase>();
        services.AddScoped<UpdateShoppingListUseCase>();
        services.AddScoped<DeleteShoppingListUseCase>();

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
        services.AddScoped<UpdateItemQuoteUseCase>();
        services.AddScoped<DeleteItemQuoteUseCase>();
    }
}
