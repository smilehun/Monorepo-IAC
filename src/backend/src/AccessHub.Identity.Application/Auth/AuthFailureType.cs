namespace AccessHub.Identity.Application.Auth;

public enum AuthFailureType
{
  Validation,
  DuplicateEmail,
  DuplicateUsername,
  InvalidCredentials,
  InvalidRefreshToken,
  UserDisabled,
  UserLocked
}
