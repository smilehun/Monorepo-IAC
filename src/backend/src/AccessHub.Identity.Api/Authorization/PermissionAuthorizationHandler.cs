using System.Security.Claims;
using AccessHub.Identity.Application.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace AccessHub.Identity.Api.Authorization;

public sealed class PermissionAuthorizationHandler(IUserPermissionReader permissionReader) : AuthorizationHandler<PermissionRequirement>
{
  protected override async Task HandleRequirementAsync(
    AuthorizationHandlerContext context,
    PermissionRequirement requirement)
  {
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
      ?? context.User.FindFirstValue("sub");

    if (!Guid.TryParse(userId, out var parsedUserId))
    {
      return;
    }

    var hasPermission = await permissionReader.HasPermissionAsync(
      parsedUserId,
      requirement.PermissionName,
      CancellationToken.None);

    if (hasPermission)
    {
      context.Succeed(requirement);
    }
  }
}
