namespace AccessHub.Identity.Application.Auth;

public sealed record GeneratedRefreshToken(string Token, string TokenHash, DateTime ExpiresAtUtc);
