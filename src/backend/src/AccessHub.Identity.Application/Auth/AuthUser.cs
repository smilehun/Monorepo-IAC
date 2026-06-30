namespace AccessHub.Identity.Application.Auth;

public sealed record AuthUser(
  Guid Id,
  string Email,
  string Username,
  string PasswordHash,
  bool IsEnabled,
  bool IsLocked);
