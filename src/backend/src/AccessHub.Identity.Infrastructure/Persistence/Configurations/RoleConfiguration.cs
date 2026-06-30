using AccessHub.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessHub.Identity.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
  public void Configure(EntityTypeBuilder<Role> builder)
  {
    builder.ToTable("roles");

    builder.HasKey(role => role.Id);

    builder.Property(role => role.Name)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(role => role.Description)
      .HasMaxLength(500);

    builder.HasIndex(role => role.Name)
      .IsUnique();
  }
}
