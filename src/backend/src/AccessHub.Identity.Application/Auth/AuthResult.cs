namespace AccessHub.Identity.Application.Auth;

public sealed record AuthResult(AuthenticationResponse? Response, AuthFailure? Failure)
{
  public bool IsSuccess => Response is not null;

  public static AuthResult Success(AuthenticationResponse response) => new(response, null);

  public static AuthResult Validation(IReadOnlyDictionary<string, string[]> errors) =>
    new(null, new AuthFailure(AuthFailureType.Validation, "One or more validation errors occurred.", errors));

  public static AuthResult DuplicateEmail() =>
    new(null, new AuthFailure(AuthFailureType.DuplicateEmail, "A user with that email already exists."));

  public static AuthResult DuplicateUsername() =>
    new(null, new AuthFailure(AuthFailureType.DuplicateUsername, "A user with that username already exists."));

  public static AuthResult InvalidCredentials() =>
    new(null, new AuthFailure(AuthFailureType.InvalidCredentials, "Invalid email or password."));

  public static AuthResult InvalidRefreshToken() =>
    new(null, new AuthFailure(AuthFailureType.InvalidRefreshToken, "Invalid refresh token."));

  public static AuthResult UserDisabled() =>
    new(null, new AuthFailure(AuthFailureType.UserDisabled, "The user account is disabled."));

  public static AuthResult UserLocked() =>
    new(null, new AuthFailure(AuthFailureType.UserLocked, "The user account is locked."));
}
