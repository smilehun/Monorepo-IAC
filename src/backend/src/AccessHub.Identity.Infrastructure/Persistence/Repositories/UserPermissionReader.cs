using AccessHub.Identity.Application.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Identity.Infrastructure.Persistence.Repositories;

public sealed class UserPermissionReader(IdentityDbContext dbContext) : IUserPermissionReader
{
  public Task<bool> HasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken)
  {
    return dbContext.UserRoles
      .AsNoTracking()
      .Where(userRole => userRole.UserId == userId && userRole.User.IsEnabled && !userRole.User.IsLocked)
      .SelectMany(userRole => userRole.Role.RolePermissions)
      .AnyAsync(rolePermission => rolePermission.Permission.Name == permissionName, cancellationToken);
  }
}
