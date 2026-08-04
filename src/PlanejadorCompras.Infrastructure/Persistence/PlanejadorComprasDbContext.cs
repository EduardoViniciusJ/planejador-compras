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

    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();

    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    public DbSet<QuotationRequest> QuotationRequests => Set<QuotationRequest>();

    public DbSet<QuotationRequestItem> QuotationRequestItems => Set<QuotationRequestItem>();

    public DbSet<SavedEqualization> SavedEqualizations => Set<SavedEqualization>();

    public DbSet<SavedEqualizationItem> SavedEqualizationItems => Set<SavedEqualizationItem>();

    public DbSet<SavedEqualizationQuote> SavedEqualizationQuotes => Set<SavedEqualizationQuote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlanejadorComprasDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
