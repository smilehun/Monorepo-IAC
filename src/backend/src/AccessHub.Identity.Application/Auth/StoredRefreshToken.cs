namespace AccessHub.Identity.Application.Auth;

public sealed record StoredRefreshToken(
  Guid Id,
  Guid UserId,
  string Email,
  string Username,
  bool IsEnabled,
  bool IsLocked,
  DateTime ExpiresAtUtc,
  DateTime? RevokedAtUtc);
