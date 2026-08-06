using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class SavedEqualizationItemConfiguration
    : IEntityTypeConfiguration<SavedEqualizationItem>
{
    public void Configure(EntityTypeBuilder<SavedEqualizationItem> builder)
    {
        builder.ToTable("EqualizationItems");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .IsRequired()
            .HasMaxLength(EqualizationRules.ItemNameMaxLength);
        builder.Property(item => item.Position)
            .IsRequired();
        builder.Property(item => item.Quantity)
            .HasPrecision(
                ShoppingItemRules.QuantityPrecision,
                ShoppingItemRules.QuantityScale)
            .IsRequired();
        builder.Property(item => item.Unit)
            .IsRequired()
            .HasMaxLength(EqualizationRules.ItemUnitMaxLength);

        builder.HasIndex(item => item.SavedEqualizationId);
        builder.HasIndex(item => new
            {
                item.SavedEqualizationId,
                item.Position
            })
            .IsUnique();

        builder.HasMany(item => item.Quotes)
            .WithOne()
            .HasForeignKey(quote => quote.SavedEqualizationItemId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(item => item.Quotes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
