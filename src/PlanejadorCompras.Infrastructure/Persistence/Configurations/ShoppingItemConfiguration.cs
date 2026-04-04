using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class ShoppingItemConfiguration : IEntityTypeConfiguration<ShoppingItem>
{
    public void Configure(EntityTypeBuilder<ShoppingItem> builder)
    {
        builder.ToTable("ShoppingItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(item => item.Unit)
            .IsRequired();

        builder.Property(item => item.Quantity)
            .HasPrecision(18, 2);

        builder.Property(item => item.CreatedAt)
            .IsRequired();

        builder.HasOne<ShoppingList>()
            .WithMany()
            .HasForeignKey(item => item.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
