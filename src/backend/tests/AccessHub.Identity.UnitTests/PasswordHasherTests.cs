using AccessHub.Identity.Infrastructure.Auth;
using Xunit;

namespace AccessHub.Identity.UnitTests;

public sealed class PasswordHasherTests
{
  [Fact]
  public void Hash_And_Verify_RoundTripSuccessfully()
  {
    var passwordHasher = new PasswordHasher();
    const string password = "Password123!";

    var hash = passwordHasher.Hash(password);

    Assert.NotEqual(password, hash);
    Assert.True(passwordHasher.Verify(password, hash));
    Assert.False(passwordHasher.Verify("wrong-password", hash));
  }
}
