using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class QuotationRequestItemConfiguration
    : IEntityTypeConfiguration<QuotationRequestItem>
{
    public void Configure(EntityTypeBuilder<QuotationRequestItem> builder)
    {
        builder.ToTable("QuotationRequestItems");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Name)
            .IsRequired()
            .HasMaxLength(QuotationRequestRules.ItemNameMaxLength);
        builder.Property(item => item.Position).IsRequired();
        builder.Property(item => item.Quantity)
            .HasPrecision(
                ShoppingItemRules.QuantityPrecision,
                ShoppingItemRules.QuantityScale)
            .IsRequired();
        builder.Property(item => item.Unit)
            .IsRequired()
            .HasMaxLength(QuotationRequestRules.ItemUnitMaxLength);
        builder.HasIndex(item => item.QuotationRequestId);
    }
}
