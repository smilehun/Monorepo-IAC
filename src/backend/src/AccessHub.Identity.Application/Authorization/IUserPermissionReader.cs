namespace AccessHub.Identity.Application.Authorization;

public interface IUserPermissionReader
{
  Task<bool> HasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken);
}
