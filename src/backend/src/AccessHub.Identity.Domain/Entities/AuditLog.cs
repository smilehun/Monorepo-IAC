namespace AccessHub.Identity.Domain.Entities;

public sealed class AuditLog
{
  public Guid Id { get; set; }

  public Guid? ActorUserId { get; set; }

  public string Action { get; set; } = string.Empty;

  public string Resource { get; set; } = string.Empty;

  public string IpAddress { get; set; } = string.Empty;

  public string Result { get; set; } = string.Empty;

  public DateTime CreatedAtUtc { get; set; }
}
