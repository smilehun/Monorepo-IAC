namespace AccessHub.Identity.Domain.Entities;

public sealed class Role
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public string? Description { get; set; }

  public ICollection<UserRole> UserRoles { get; set; } = [];

  public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
