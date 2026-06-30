using Microsoft.AspNetCore.Authorization;

namespace AccessHub.Identity.Api.Authorization;

public sealed class PermissionRequirement(string permissionName) : IAuthorizationRequirement
{
  public string PermissionName { get; } = permissionName;
}
