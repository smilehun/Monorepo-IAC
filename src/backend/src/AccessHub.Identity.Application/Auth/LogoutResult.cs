namespace AccessHub.Identity.Application.Auth;

public sealed record LogoutResult(IReadOnlyDictionary<string, string[]>? ValidationErrors = null)
{
  public bool IsSuccess => ValidationErrors is null;

  public static LogoutResult Success() => new();

  public static LogoutResult Validation(IReadOnlyDictionary<string, string[]> errors) => new(errors);
}
