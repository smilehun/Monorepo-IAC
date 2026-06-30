using System.ComponentModel.DataAnnotations;

namespace AccessHub.Identity.Application.Auth;

internal static class AuthInputValidator
{
  private static readonly EmailAddressAttribute EmailValidator = new();

  public static IReadOnlyDictionary<string, string[]> Validate(RegisterUserCommand command)
  {
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(command.Email))
    {
      errors[nameof(command.Email)] = ["Email is required."];
    }
    else if (!EmailValidator.IsValid(command.Email))
    {
      errors[nameof(command.Email)] = ["Email must be a valid email address."];
    }

    if (string.IsNullOrWhiteSpace(command.Username))
    {
      errors[nameof(command.Username)] = ["Username is required."];
    }
    else if (command.Username.Trim().Length < 3)
    {
      errors[nameof(command.Username)] = ["Username must be at least 3 characters long."];
    }

    if (string.IsNullOrWhiteSpace(command.Password))
    {
      errors[nameof(command.Password)] = ["Password is required."];
    }
    else if (command.Password.Length < 8)
    {
      errors[nameof(command.Password)] = ["Password must be at least 8 characters long."];
    }

    return errors;
  }

  public static IReadOnlyDictionary<string, string[]> Validate(LoginUserCommand command)
  {
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(command.Email))
    {
      errors[nameof(command.Email)] = ["Email is required."];
    }
    else if (!EmailValidator.IsValid(command.Email))
    {
      errors[nameof(command.Email)] = ["Email must be a valid email address."];
    }

    if (string.IsNullOrWhiteSpace(command.Password))
    {
      errors[nameof(command.Password)] = ["Password is required."];
    }

    return errors;
  }

  public static IReadOnlyDictionary<string, string[]> Validate(RefreshTokenCommand command)
  {
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(command.RefreshToken))
    {
      errors[nameof(command.RefreshToken)] = ["Refresh token is required."];
    }

    return errors;
  }

  public static IReadOnlyDictionary<string, string[]> Validate(LogoutCommand command)
  {
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(command.RefreshToken))
    {
      errors[nameof(command.RefreshToken)] = ["Refresh token is required."];
    }

    return errors;
  }

  public static IReadOnlyDictionary<string, string[]> Validate(ChangePasswordCommand command)
  {
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(command.CurrentPassword))
    {
      errors[nameof(command.CurrentPassword)] = ["Current password is required."];
    }

    if (string.IsNullOrWhiteSpace(command.NewPassword))
    {
      errors[nameof(command.NewPassword)] = ["New password is required."];
    }
    else if (command.NewPassword.Length < 8)
    {
      errors[nameof(command.NewPassword)] = ["New password must be at least 8 characters long."];
    }
    else if (command.NewPassword == command.CurrentPassword)
    {
      errors[nameof(command.NewPassword)] = ["New password must be different from current password."];
    }

    return errors;
  }

  public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

  public static string NormalizeUsername(string username) => username.Trim();
}
