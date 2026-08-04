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

        builder.Property(supplier => supplier.Cnpj)
            .HasMaxLength(14);

        builder.Property(supplier => supplier.CreatedAt)
            .IsRequired();

        builder.HasIndex(supplier => new { supplier.UserId, supplier.Name })
            .IsUnique();

        builder.HasIndex(supplier => new { supplier.UserId, supplier.Cnpj })
            .IsUnique()
            .HasFilter("[Cnpj] IS NOT NULL");

        builder.OwnsOne(supplier => supplier.Address, address =>
        {
            address.Property(value => value.Street)
                .HasColumnName("AddressStreet")
                .HasMaxLength(200);
            address.Property(value => value.City)
                .HasColumnName("AddressCity")
                .HasMaxLength(100);
            address.Property(value => value.PostalCode)
                .HasColumnName("AddressPostalCode")
                .HasMaxLength(8);
        });

        builder.OwnsOne(supplier => supplier.Contact, contact =>
        {
            contact.Property(value => value.Email)
                .HasColumnName("ContactEmail")
                .HasMaxLength(254);
            contact.Property(value => value.Phone)
                .HasColumnName("ContactPhone")
                .HasMaxLength(13);
        });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(supplier => supplier.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
