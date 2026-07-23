using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class ShoppingListSupplierConfiguration : IEntityTypeConfiguration<ShoppingListSupplier>
{
    public void Configure(EntityTypeBuilder<ShoppingListSupplier> builder)
    {
        builder.ToTable("ShoppingListSuppliers");

        builder.HasKey(link => new { link.ShoppingListId, link.SupplierId });

        builder.Property(link => link.CreatedAt)
            .IsRequired();

        builder.HasOne<ShoppingList>()
            .WithMany()
            .HasForeignKey(link => link.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Supplier>()
            .WithMany()
            .HasForeignKey(link => link.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
