using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class ItemQuoteConfiguration : IEntityTypeConfiguration<ItemQuote>
{
    public void Configure(EntityTypeBuilder<ItemQuote> builder)
    {
        builder.ToTable("ItemQuotes");

        builder.HasKey(quote => quote.Id);

        builder.Property(quote => quote.UnitPrice)
            .HasPrecision(
                ItemQuoteRules.UnitPricePrecision,
                ItemQuoteRules.UnitPriceScale);

        builder.Property(quote => quote.CreatedAt)
            .IsRequired();

        builder.HasOne<ShoppingItem>()
            .WithMany()
            .HasForeignKey(quote => quote.ShoppingItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Supplier>()
            .WithMany()
            .HasForeignKey(quote => quote.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
