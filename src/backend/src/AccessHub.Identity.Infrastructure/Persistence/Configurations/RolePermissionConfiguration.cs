using AccessHub.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessHub.Identity.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
  public void Configure(EntityTypeBuilder<RolePermission> builder)
  {
    builder.ToTable("role_permissions");

    builder.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId });

    builder.HasOne(rolePermission => rolePermission.Role)
      .WithMany(role => role.RolePermissions)
      .HasForeignKey(rolePermission => rolePermission.RoleId);

    builder.HasOne(rolePermission => rolePermission.Permission)
      .WithMany(permission => permission.RolePermissions)
      .HasForeignKey(rolePermission => rolePermission.PermissionId);
  }
}
