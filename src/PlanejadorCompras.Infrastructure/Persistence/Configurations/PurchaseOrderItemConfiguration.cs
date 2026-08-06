using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class PurchaseOrderItemConfiguration
    : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .IsRequired()
            .HasMaxLength(PurchaseOrderRules.ItemNameMaxLength);
        builder.Property(item => item.Quantity)
            .HasPrecision(
                ShoppingItemRules.QuantityPrecision,
                ShoppingItemRules.QuantityScale)
            .IsRequired();
        builder.Property(item => item.Unit)
            .IsRequired()
            .HasMaxLength(PurchaseOrderRules.ItemUnitMaxLength);
        builder.Property(item => item.UnitPrice)
            .HasPrecision(
                ItemQuoteRules.UnitPricePrecision,
                ItemQuoteRules.UnitPriceScale)
            .IsRequired();

        builder.Ignore(item => item.TotalPrice);
        builder.HasIndex(item => item.PurchaseOrderId);
    }
}
