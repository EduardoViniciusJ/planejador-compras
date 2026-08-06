using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanejadorCompras.Domain.Entities;
using PlanejadorCompras.Domain.Rules;

namespace PlanejadorCompras.Infrastructure.Persistence.Configurations;

public sealed class ShoppingListConfiguration : IEntityTypeConfiguration<ShoppingList>
{
    public void Configure(EntityTypeBuilder<ShoppingList> builder)
    {
        builder.ToTable("ShoppingLists");

        builder.HasKey(list => list.Id);

        builder.Property(list => list.Name)
            .IsRequired()
            .HasMaxLength(ShoppingListRules.NameMaxLength);

        builder.Property(list => list.Description)
            .HasMaxLength(ShoppingListRules.DescriptionMaxLength);

        builder.Property(list => list.CreatedAt)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(list => list.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
