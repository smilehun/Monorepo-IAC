namespace AccessHub.Identity.Application.Auth;

public sealed record AuthenticationTokens(
  string AccessToken,
  DateTime AccessTokenExpiresAtUtc,
  string RefreshToken,
  DateTime RefreshTokenExpiresAtUtc);
