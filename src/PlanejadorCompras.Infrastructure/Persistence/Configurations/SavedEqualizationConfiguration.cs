using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class SavedEqualizationConfiguration
    : IEntityTypeConfiguration<SavedEqualization>
{
    public void Configure(EntityTypeBuilder<SavedEqualization> builder)
    {
        builder.ToTable("Equalizations");
        builder.HasKey(equalization => equalization.Id);

        builder.Property(equalization => equalization.Code)
            .IsRequired()
            .HasMaxLength(EqualizationRules.CodeMaxLength);
        builder.Property(equalization => equalization.ShoppingListName)
            .IsRequired()
            .HasMaxLength(EqualizationRules.ShoppingListNameMaxLength);
        builder.Property(equalization => equalization.CreatedByName)
            .IsRequired()
            .HasMaxLength(EqualizationRules.CreatedByNameMaxLength);
        builder.Property(equalization => equalization.CreatedByEmail)
            .IsRequired()
            .HasMaxLength(EqualizationRules.CreatedByEmailMaxLength);
        builder.Property(equalization => equalization.BestChoiceTotal)
            .HasPrecision(18, 2)
            .IsRequired();
        builder.Property(equalization => equalization.BestCompleteSupplierName)
            .HasMaxLength(EqualizationRules.SupplierNameMaxLength);
        builder.Property(equalization => equalization.BestCompleteSupplierTotal)
            .HasPrecision(18, 2);
        builder.Property(equalization => equalization.EstimatedEconomy)
            .HasPrecision(18, 2)
            .IsRequired();
        builder.Property(equalization => equalization.CreatedAtUtc)
            .IsRequired();

        builder.Ignore(equalization => equalization.SupplierCount);

        builder.HasIndex(equalization => equalization.Code)
            .IsUnique();
        builder.HasIndex(equalization => new
            {
                equalization.UserId,
                equalization.RequestId
            })
            .IsUnique();
        builder.HasIndex(equalization => new
            {
                equalization.UserId,
                equalization.CreatedAtUtc
            });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(equalization => equalization.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // SourceShoppingListId is an audit reference. The immutable snapshot
        // must remain readable even if the source list is removed later.
        builder.HasMany(equalization => equalization.Items)
            .WithOne()
            .HasForeignKey(item => item.SavedEqualizationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(equalization => equalization.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
