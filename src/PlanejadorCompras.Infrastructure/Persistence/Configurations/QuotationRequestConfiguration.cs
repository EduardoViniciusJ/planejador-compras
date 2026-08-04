using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class QuotationRequestConfiguration : IEntityTypeConfiguration<QuotationRequest>
{
    public void Configure(EntityTypeBuilder<QuotationRequest> builder)
    {
        builder.ToTable("QuotationRequests");
        builder.HasKey(request => request.Id);
        builder.Property(request => request.Code).IsRequired().HasMaxLength(32);
        builder.Property(request => request.ShoppingListName).IsRequired().HasMaxLength(150);
        builder.Property(request => request.Description).HasMaxLength(500);
        builder.Property(request => request.BuyerName).IsRequired().HasMaxLength(150);
        builder.Property(request => request.BuyerEmail).IsRequired().HasMaxLength(320);
        builder.Property(request => request.ResponseDeadline).HasColumnType("date");
        builder.Property(request => request.DeliveryAddress).HasMaxLength(500);
        builder.Property(request => request.Instructions).HasMaxLength(2000);
        builder.Property(request => request.CreatedAtUtc).IsRequired();
        builder.HasIndex(request => request.Code).IsUnique();
        builder.HasIndex(request => new { request.UserId, request.CreatedAtUtc });
        builder.HasOne<User>().WithMany().HasForeignKey(request => request.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(request => request.Items)
            .WithOne()
            .HasForeignKey(item => item.QuotationRequestId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(request => request.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class QuotationRequestItemConfiguration : IEntityTypeConfiguration<QuotationRequestItem>
{
    public void Configure(EntityTypeBuilder<QuotationRequestItem> builder)
    {
        builder.ToTable("QuotationRequestItems");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Name).IsRequired().HasMaxLength(100);
        builder.Property(item => item.Position).IsRequired();
        builder.Property(item => item.Quantity).HasPrecision(18, 3).IsRequired();
        builder.Property(item => item.Unit).IsRequired().HasMaxLength(20);
        builder.HasIndex(item => item.QuotationRequestId);
    }
}
