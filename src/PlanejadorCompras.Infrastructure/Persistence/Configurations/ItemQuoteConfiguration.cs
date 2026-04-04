using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class ItemQuoteConfiguration : IEntityTypeConfiguration<ItemQuote>
{
    public void Configure(EntityTypeBuilder<ItemQuote> builder)
    {
        builder.ToTable("ItemQuotes");

        builder.HasKey(quote => quote.Id);

        builder.Property(quote => quote.SupplierName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(quote => quote.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(quote => quote.CreatedAt)
            .IsRequired();

        builder.HasOne<ShoppingItem>()
            .WithMany()
            .HasForeignKey(quote => quote.ShoppingItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
