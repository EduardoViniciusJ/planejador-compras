using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class ShoppingItemConfiguration : IEntityTypeConfiguration<ShoppingItem>
{
    public void Configure(EntityTypeBuilder<ShoppingItem> builder)
    {
        builder.ToTable("ShoppingItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .IsRequired()
            .HasMaxLength(ShoppingItemRules.NameMaxLength);

        builder.Property(item => item.Unit)
            .IsRequired()
            .HasMaxLength(ShoppingItemRules.UnitMaxLength);

        builder.Property(item => item.Quantity)
            .HasPrecision(
                ShoppingItemRules.QuantityPrecision,
                ShoppingItemRules.QuantityScale);

        builder.Property(item => item.CreatedAt)
            .IsRequired();

        builder.HasOne<ShoppingList>()
            .WithMany()
            .HasForeignKey(item => item.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
