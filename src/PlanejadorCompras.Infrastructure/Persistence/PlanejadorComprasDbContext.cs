using Microsoft.EntityFrameworkCore;
using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Infrastructure.Persistence;

public sealed class PlanejadorComprasDbContext(DbContextOptions<PlanejadorComprasDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();

    public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();

    public DbSet<ItemQuote> ItemQuotes => Set<ItemQuote>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<ShoppingListSupplier> ShoppingListSuppliers => Set<ShoppingListSupplier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlanejadorComprasDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
