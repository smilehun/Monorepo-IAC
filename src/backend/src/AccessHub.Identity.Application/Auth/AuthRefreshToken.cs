namespace AccessHub.Identity.Application.Auth;

public sealed record AuthRefreshToken(Guid Id, Guid UserId, string TokenHash, DateTime CreatedAtUtc, DateTime ExpiresAtUtc);
