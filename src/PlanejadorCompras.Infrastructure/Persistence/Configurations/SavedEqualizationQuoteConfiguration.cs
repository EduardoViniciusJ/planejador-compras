using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class SavedEqualizationQuoteConfiguration
    : IEntityTypeConfiguration<SavedEqualizationQuote>
{
    public void Configure(EntityTypeBuilder<SavedEqualizationQuote> builder)
    {
        builder.ToTable("EqualizationQuotes");
        builder.HasKey(quote => quote.Id);

        builder.Property(quote => quote.SupplierName)
            .IsRequired()
            .HasMaxLength(EqualizationRules.SupplierNameMaxLength);
        builder.Property(quote => quote.UnitPrice)
            .HasPrecision(
                ItemQuoteRules.UnitPricePrecision,
                ItemQuoteRules.UnitPriceScale)
            .IsRequired();
        builder.Property(quote => quote.IsLowest)
            .IsRequired();

        builder.HasIndex(quote => quote.SavedEqualizationItemId);
        builder.HasIndex(quote => new
            {
                quote.SavedEqualizationItemId,
                quote.SourceSupplierId
            })
            .IsUnique();
    }
}
