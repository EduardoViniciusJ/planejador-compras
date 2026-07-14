using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");
        builder.HasKey(supplier => supplier.Id);

        builder.Property(supplier => supplier.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(supplier => supplier.CreatedAt)
            .IsRequired();

        builder.HasIndex(supplier => new { supplier.UserId, supplier.Name })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(supplier => supplier.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
