namespace AccessHub.Identity.Domain.Entities;

public sealed class RefreshToken
{
  public Guid Id { get; set; }

  public Guid UserId { get; set; }

  public string TokenHash { get; set; } = string.Empty;

  public DateTime ExpiresAtUtc { get; set; }

  public DateTime CreatedAtUtc { get; set; }

  public DateTime? RevokedAtUtc { get; set; }

  public Guid? ReplacedByTokenId { get; set; }

  public User User { get; set; } = null!;

  public RefreshToken? ReplacedByToken { get; set; }
}
