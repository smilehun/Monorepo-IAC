using AccessHub.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
  public DbSet<User> Users => Set<User>();

  public DbSet<Role> Roles => Set<Role>();

  public DbSet<Permission> Permissions => Set<Permission>();

  public DbSet<UserRole> UserRoles => Set<UserRole>();

  public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

  public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
  }
}
