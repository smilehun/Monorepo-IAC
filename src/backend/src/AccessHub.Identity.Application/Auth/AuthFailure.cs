namespace AccessHub.Identity.Application.Auth;

public sealed record AuthFailure(
  AuthFailureType Type,
  string Message,
  IReadOnlyDictionary<string, string[]>? ValidationErrors = null);
