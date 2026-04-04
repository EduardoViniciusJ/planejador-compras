using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.GoogleId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.HasIndex(user => user.GoogleId)
            .IsUnique();

        builder.HasIndex(user => user.Email)
            .IsUnique();
    }
}
