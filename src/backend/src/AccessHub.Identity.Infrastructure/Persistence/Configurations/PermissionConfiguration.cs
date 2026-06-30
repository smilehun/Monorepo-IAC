using AccessHub.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessHub.Identity.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
  public void Configure(EntityTypeBuilder<Permission> builder)
  {
    builder.ToTable("permissions");

    builder.HasKey(permission => permission.Id);

    builder.Property(permission => permission.Name)
      .HasMaxLength(150)
      .IsRequired();

    builder.HasIndex(permission => permission.Name)
      .IsUnique();
  }
}
