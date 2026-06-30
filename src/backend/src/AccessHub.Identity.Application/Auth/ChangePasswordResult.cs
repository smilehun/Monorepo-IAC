namespace AccessHub.Identity.Application.Auth;

public sealed record ChangePasswordResult(AuthFailure? Failure = null)
{
  public bool IsSuccess => Failure is null;

  public static ChangePasswordResult Success() => new();

  public static ChangePasswordResult Validation(IReadOnlyDictionary<string, string[]> errors) =>
    new(new AuthFailure(AuthFailureType.Validation, "One or more validation errors occurred.", errors));

  public static ChangePasswordResult InvalidCredentials() =>
    new(new AuthFailure(AuthFailureType.InvalidCredentials, "Invalid current password."));

  public static ChangePasswordResult UserDisabled() =>
    new(new AuthFailure(AuthFailureType.UserDisabled, "The user account is disabled."));

  public static ChangePasswordResult UserLocked() =>
    new(new AuthFailure(AuthFailureType.UserLocked, "The user account is locked."));
}
