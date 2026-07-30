using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");
        builder.HasKey(order => order.Id);

        builder.Property(order => order.Code)
            .IsRequired()
            .HasMaxLength(32);
        builder.Property(order => order.ShoppingListName)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(order => order.SupplierName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(order => order.BuyerName)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(order => order.BuyerEmail)
            .HasMaxLength(320);
        builder.Property(order => order.ExpectedDeliveryDate)
            .HasColumnType("date");
        builder.Property(order => order.DeliveryAddress)
            .HasMaxLength(500);
        builder.Property(order => order.PaymentTerms)
            .HasMaxLength(200);
        builder.Property(order => order.Notes)
            .HasMaxLength(1000);
        builder.Property(order => order.Status)
            .IsRequired();
        builder.Property(order => order.CreatedAtUtc)
            .IsRequired();
        builder.Property(order => order.UpdatedAtUtc)
            .IsRequired();

        builder.Ignore(order => order.TotalPrice);

        builder.HasIndex(order => order.Code)
            .IsUnique();
        builder.HasIndex(order => new
            {
                order.UserId,
                order.SourceShoppingListId,
                order.SupplierId
            })
            .IsUnique()
            .HasFilter(
                "[Status] <> 3 AND [SourceShoppingListId] IS NOT NULL AND [SupplierId] IS NOT NULL");
        builder.HasIndex(order => new { order.UserId, order.CreatedAtUtc });
        builder.HasIndex(order => order.SourceEqualizationId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(order => order.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Source IDs are audit references rather than aggregate relationships.
        // The validated snapshots keep an issued order readable after its list or
        // supplier is deleted, without creating SQL Server cascade paths.
        builder.HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(item => item.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(order => order.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
