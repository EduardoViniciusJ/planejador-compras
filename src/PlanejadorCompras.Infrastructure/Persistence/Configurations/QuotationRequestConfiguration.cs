using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class QuotationRequestConfiguration : IEntityTypeConfiguration<QuotationRequest>
{
    public void Configure(EntityTypeBuilder<QuotationRequest> builder)
    {
        builder.ToTable("QuotationRequests");
        builder.HasKey(request => request.Id);
        builder.Property(request => request.Code)
            .IsRequired()
            .HasMaxLength(QuotationRequestRules.CodeMaxLength);
        builder.Property(request => request.ShoppingListName)
            .IsRequired()
            .HasMaxLength(QuotationRequestRules.ShoppingListNameMaxLength);
        builder.Property(request => request.Description)
            .HasMaxLength(QuotationRequestRules.DescriptionMaxLength);
        builder.Property(request => request.BuyerName)
            .IsRequired()
            .HasMaxLength(QuotationRequestRules.BuyerNameMaxLength);
        builder.Property(request => request.BuyerEmail)
            .IsRequired()
            .HasMaxLength(QuotationRequestRules.BuyerEmailMaxLength);
        builder.Property(request => request.ResponseDeadline).HasColumnType("date");
        builder.Property(request => request.DeliveryAddress)
            .HasMaxLength(QuotationRequestRules.DeliveryAddressMaxLength);
        builder.Property(request => request.Instructions)
            .HasMaxLength(QuotationRequestRules.InstructionsMaxLength);
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
