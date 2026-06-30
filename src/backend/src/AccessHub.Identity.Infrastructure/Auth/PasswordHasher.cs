using AccessHub.Identity.Application.Auth;
using Microsoft.AspNetCore.Identity;

namespace AccessHub.Identity.Infrastructure.Auth;

public sealed class PasswordHasher : IPasswordHasher
{
  private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> passwordHasher = new();

  public string Hash(string password)
  {
    return passwordHasher.HashPassword(null!, password);
  }

  public bool Verify(string password, string passwordHash)
  {
    var result = passwordHasher.VerifyHashedPassword(null!, passwordHash, password);

    return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
  }
}
