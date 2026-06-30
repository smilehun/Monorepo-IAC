using AccessHub.Identity.Application.Auth;
using Xunit;

namespace AccessHub.Identity.UnitTests;

public sealed class AuthenticationServiceTests
{
  [Fact]
  public async Task RegisterAsync_ReturnsValidationFailure_WhenInputIsInvalid()
  {
    var service = CreateService();

    var result = await service.RegisterAsync(new RegisterUserCommand("", "ab", "123"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.Validation, result.Failure?.Type);
    Assert.NotNull(result.Failure?.ValidationErrors);
    Assert.Contains(nameof(RegisterUserCommand.Email), result.Failure!.ValidationErrors!.Keys);
    Assert.Contains(nameof(RegisterUserCommand.Username), result.Failure.ValidationErrors.Keys);
    Assert.Contains(nameof(RegisterUserCommand.Password), result.Failure.ValidationErrors.Keys);
  }

  [Fact]
  public async Task RegisterAsync_ReturnsDuplicateEmail_WhenEmailAlreadyExists()
  {
    var userRepository = new FakeAuthUserRepository { EmailExists = true };
    var service = CreateService(userRepository: userRepository);

    var result = await service.RegisterAsync(new RegisterUserCommand("user@example.com", "user1", "Password123"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.DuplicateEmail, result.Failure?.Type);
  }

  [Fact]
  public async Task RegisterAsync_HashesPassword_AndPersistsRefreshToken()
  {
    var userRepository = new FakeAuthUserRepository();
    var refreshTokenRepository = new FakeRefreshTokenRepository();
    var passwordHasher = new FakePasswordHasher();
    var service = CreateService(userRepository, refreshTokenRepository, passwordHasher);

    var result = await service.RegisterAsync(new RegisterUserCommand("User@Example.com", "user1", "Password123"), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal("hashed::Password123", userRepository.LastAddedUser?.PasswordHash);
    Assert.Equal("user@example.com", userRepository.LastAddedUser?.Email);
    Assert.NotNull(refreshTokenRepository.LastAddedRefreshToken);
    Assert.Equal("refresh-token-hash", refreshTokenRepository.LastAddedRefreshToken?.TokenHash);
    Assert.Equal("refresh-token-raw", result.Response?.Tokens.RefreshToken);
  }

  [Fact]
  public async Task LoginAsync_ReturnsInvalidCredentials_WhenPasswordDoesNotMatch()
  {
    var userRepository = new FakeAuthUserRepository
    {
      User = new AuthUser(Guid.NewGuid(), "user@example.com", "user1", "hashed::Password123", true, false)
    };

    var service = CreateService(userRepository: userRepository);

    var result = await service.LoginAsync(new LoginUserCommand("user@example.com", "wrong-password"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.InvalidCredentials, result.Failure?.Type);
  }

  [Fact]
  public async Task LoginAsync_ReturnsUserDisabled_WhenUserIsDisabled()
  {
    var userRepository = new FakeAuthUserRepository
    {
      User = new AuthUser(Guid.NewGuid(), "user@example.com", "user1", "hashed::Password123", false, false)
    };

    var service = CreateService(userRepository: userRepository);

    var result = await service.LoginAsync(new LoginUserCommand("user@example.com", "Password123"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.UserDisabled, result.Failure?.Type);
  }

  [Fact]
  public async Task LoginAsync_ReturnsUserLocked_WhenUserIsLocked()
  {
    var userRepository = new FakeAuthUserRepository
    {
      User = new AuthUser(Guid.NewGuid(), "user@example.com", "user1", "hashed::Password123", true, true)
    };

    var service = CreateService(userRepository: userRepository);

    var result = await service.LoginAsync(new LoginUserCommand("user@example.com", "Password123"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.UserLocked, result.Failure?.Type);
  }

  [Fact]
  public async Task LoginAsync_ReturnsTokens_WhenCredentialsAreValid()
  {
    var userRepository = new FakeAuthUserRepository
    {
      User = new AuthUser(Guid.NewGuid(), "user@example.com", "user1", "hashed::Password123", true, false)
    };

    var service = CreateService(userRepository: userRepository);

    var result = await service.LoginAsync(new LoginUserCommand("user@example.com", "Password123"), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal("access-token-raw", result.Response?.Tokens.AccessToken);
    Assert.Equal("refresh-token-raw", result.Response?.Tokens.RefreshToken);
  }

  [Fact]
  public async Task RefreshAsync_ReturnsValidationFailure_WhenRefreshTokenIsBlank()
  {
    var service = CreateService();

    var result = await service.RefreshAsync(new RefreshTokenCommand(" "), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.Validation, result.Failure?.Type);
    Assert.Contains(nameof(RefreshTokenCommand.RefreshToken), result.Failure!.ValidationErrors!.Keys);
  }

  [Fact]
  public async Task RefreshAsync_ReturnsInvalidRefreshToken_WhenTokenIsUnknown()
  {
    var refreshTokenRepository = new FakeRefreshTokenRepository();
    var service = CreateService(refreshTokenRepository: refreshTokenRepository);

    var result = await service.RefreshAsync(new RefreshTokenCommand("refresh-token-raw"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.InvalidRefreshToken, result.Failure?.Type);
  }

  [Fact]
  public async Task RefreshAsync_ReturnsInvalidRefreshToken_WhenTokenIsExpired()
  {
    var refreshTokenRepository = new FakeRefreshTokenRepository
    {
      StoredRefreshToken = new StoredRefreshToken(Guid.NewGuid(), Guid.NewGuid(), "user@example.com", "user1", true, false, DateTime.UtcNow.AddMinutes(-1), null)
    };

    var service = CreateService(refreshTokenRepository: refreshTokenRepository);

    var result = await service.RefreshAsync(new RefreshTokenCommand("refresh-token-raw"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.InvalidRefreshToken, result.Failure?.Type);
  }

  [Fact]
  public async Task RefreshAsync_ReturnsInvalidRefreshToken_WhenTokenIsRevoked()
  {
    var refreshTokenRepository = new FakeRefreshTokenRepository
    {
      StoredRefreshToken = new StoredRefreshToken(Guid.NewGuid(), Guid.NewGuid(), "user@example.com", "user1", true, false, DateTime.UtcNow.AddMinutes(10), DateTime.UtcNow)
    };

    var service = CreateService(refreshTokenRepository: refreshTokenRepository);

    var result = await service.RefreshAsync(new RefreshTokenCommand("refresh-token-raw"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.InvalidRefreshToken, result.Failure?.Type);
  }

  [Fact]
  public async Task RefreshAsync_ReturnsUserDisabled_WhenUserIsDisabled()
  {
    var refreshTokenRepository = new FakeRefreshTokenRepository
    {
      StoredRefreshToken = new StoredRefreshToken(Guid.NewGuid(), Guid.NewGuid(), "user@example.com", "user1", false, false, DateTime.UtcNow.AddMinutes(10), null)
    };

    var service = CreateService(refreshTokenRepository: refreshTokenRepository);

    var result = await service.RefreshAsync(new RefreshTokenCommand("refresh-token-raw"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.UserDisabled, result.Failure?.Type);
  }

  [Fact]
  public async Task RefreshAsync_ReturnsUserLocked_WhenUserIsLocked()
  {
    var refreshTokenRepository = new FakeRefreshTokenRepository
    {
      StoredRefreshToken = new StoredRefreshToken(Guid.NewGuid(), Guid.NewGuid(), "user@example.com", "user1", true, true, DateTime.UtcNow.AddMinutes(10), null)
    };

    var service = CreateService(refreshTokenRepository: refreshTokenRepository);

    var result = await service.RefreshAsync(new RefreshTokenCommand("refresh-token-raw"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.UserLocked, result.Failure?.Type);
  }

  [Fact]
  public async Task RefreshAsync_RotatesRefreshToken_AndReturnsNewTokens_WhenTokenIsValid()
  {
    var currentTokenId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var refreshTokenRepository = new FakeRefreshTokenRepository
    {
      StoredRefreshToken = new StoredRefreshToken(currentTokenId, userId, "user@example.com", "user1", true, false, DateTime.UtcNow.AddMinutes(10), null)
    };

    var refreshTokenService = new FakeRefreshTokenService();
    var service = CreateService(refreshTokenRepository: refreshTokenRepository, refreshTokenService: refreshTokenService);

    var result = await service.RefreshAsync(new RefreshTokenCommand("refresh-token-raw"), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal("hashed::refresh-token-raw", refreshTokenRepository.LastLookupTokenHash);
    Assert.NotNull(refreshTokenRepository.LastRotatedToken);
    Assert.Equal(currentTokenId, refreshTokenRepository.LastRotatedTokenId);
    Assert.Equal(userId, refreshTokenRepository.LastRotatedToken!.UserId);
    Assert.Equal("refresh-token-hash", refreshTokenRepository.LastRotatedToken.TokenHash);
    Assert.Equal("refresh-token-raw", result.Response?.Tokens.RefreshToken);
    Assert.Equal("access-token-raw", result.Response?.Tokens.AccessToken);
  }

  [Fact]
  public async Task ChangePasswordAsync_ReturnsValidationFailure_WhenCurrentPasswordIsBlank()
  {
    var service = CreateService();

    var result = await service.ChangePasswordAsync(new ChangePasswordCommand(Guid.NewGuid(), " ", "Password456"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.Validation, result.Failure?.Type);
    Assert.Contains(nameof(ChangePasswordCommand.CurrentPassword), result.Failure!.ValidationErrors!.Keys);
  }

  [Fact]
  public async Task ChangePasswordAsync_ReturnsValidationFailure_WhenNewPasswordIsInvalid()
  {
    var service = CreateService();

    var result = await service.ChangePasswordAsync(new ChangePasswordCommand(Guid.NewGuid(), "Password123", "short"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.Validation, result.Failure?.Type);
    Assert.Contains(nameof(ChangePasswordCommand.NewPassword), result.Failure!.ValidationErrors!.Keys);
  }

  [Fact]
  public async Task ChangePasswordAsync_ReturnsValidationFailure_WhenNewPasswordMatchesCurrentPassword()
  {
    var service = CreateService();

    var result = await service.ChangePasswordAsync(new ChangePasswordCommand(Guid.NewGuid(), "Password123", "Password123"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.Validation, result.Failure?.Type);
    Assert.Contains(nameof(ChangePasswordCommand.NewPassword), result.Failure!.ValidationErrors!.Keys);
  }

  [Fact]
  public async Task ChangePasswordAsync_ReturnsInvalidCredentials_WhenUserDoesNotExist()
  {
    var service = CreateService(userRepository: new FakeAuthUserRepository());

    var result = await service.ChangePasswordAsync(new ChangePasswordCommand(Guid.NewGuid(), "Password123", "Password456"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.InvalidCredentials, result.Failure?.Type);
  }

  [Fact]
  public async Task ChangePasswordAsync_ReturnsInvalidCredentials_WhenCurrentPasswordIsWrong()
  {
    var user = new AuthUser(Guid.NewGuid(), "user@example.com", "user1", "hashed::Password123", true, false);
    var userRepository = new FakeAuthUserRepository { User = user };
    var service = CreateService(userRepository: userRepository);

    var result = await service.ChangePasswordAsync(new ChangePasswordCommand(user.Id, "wrong-password", "Password456"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.InvalidCredentials, result.Failure?.Type);
  }

  [Fact]
  public async Task ChangePasswordAsync_ReturnsUserDisabled_WhenUserIsDisabled()
  {
    var user = new AuthUser(Guid.NewGuid(), "user@example.com", "user1", "hashed::Password123", false, false);
    var userRepository = new FakeAuthUserRepository { User = user };
    var service = CreateService(userRepository: userRepository);

    var result = await service.ChangePasswordAsync(new ChangePasswordCommand(user.Id, "Password123", "Password456"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.UserDisabled, result.Failure?.Type);
  }

  [Fact]
  public async Task ChangePasswordAsync_ReturnsUserLocked_WhenUserIsLocked()
  {
    var user = new AuthUser(Guid.NewGuid(), "user@example.com", "user1", "hashed::Password123", true, true);
    var userRepository = new FakeAuthUserRepository { User = user };
    var service = CreateService(userRepository: userRepository);

    var result = await service.ChangePasswordAsync(new ChangePasswordCommand(user.Id, "Password123", "Password456"), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(AuthFailureType.UserLocked, result.Failure?.Type);
  }

  [Fact]
  public async Task ChangePasswordAsync_UpdatesPasswordHash_AndRevokesRefreshTokens()
  {
    var user = new AuthUser(Guid.NewGuid(), "user@example.com", "user1", "hashed::Password123", true, false);
    var userRepository = new FakeAuthUserRepository { User = user };
    var service = CreateService(userRepository: userRepository);

    var result = await service.ChangePasswordAsync(new ChangePasswordCommand(user.Id, "Password123", "Password456"), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal(user.Id, userRepository.LastChangedPasswordUserId);
    Assert.Equal("hashed::Password456", userRepository.LastChangedPasswordHash);
    Assert.NotNull(userRepository.LastChangedPasswordAtUtc);
  }

  [Fact]
  public async Task LogoutAsync_ReturnsValidationFailure_WhenRefreshTokenIsBlank()
  {
    var service = CreateService();

    var result = await service.LogoutAsync(new LogoutCommand(" "), CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Contains(nameof(LogoutCommand.RefreshToken), result.ValidationErrors!.Keys);
  }

  [Fact]
  public async Task LogoutAsync_HashesToken_AndRevokesMatchingRefreshToken()
  {
    var refreshTokenRepository = new FakeRefreshTokenRepository();
    var service = CreateService(refreshTokenRepository: refreshTokenRepository);

    var result = await service.LogoutAsync(new LogoutCommand("refresh-token-raw"), CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal("hashed::refresh-token-raw", refreshTokenRepository.LastRevokedTokenHash);
    Assert.NotNull(refreshTokenRepository.LastRevokedAtUtc);
    Assert.Null(refreshTokenRepository.LastRotatedToken);
  }

  [Fact]
  public async Task LogoutAsync_ReturnsSuccess_WhenTokenIsUnknown()
  {
    var service = CreateService(refreshTokenRepository: new FakeRefreshTokenRepository());

    var result = await service.LogoutAsync(new LogoutCommand("unknown-token"), CancellationToken.None);

    Assert.True(result.IsSuccess);
  }

  private static IAuthenticationService CreateService(
    FakeAuthUserRepository? userRepository = null,
    FakeRefreshTokenRepository? refreshTokenRepository = null,
    FakePasswordHasher? passwordHasher = null,
    FakeRefreshTokenService? refreshTokenService = null)
  {
    return new AuthenticationService(
      userRepository ?? new FakeAuthUserRepository(),
      refreshTokenRepository ?? new FakeRefreshTokenRepository(),
      passwordHasher ?? new FakePasswordHasher(),
      new FakeAccessTokenService(),
      refreshTokenService ?? new FakeRefreshTokenService());
  }

  private sealed class FakeAuthUserRepository : IAuthUserRepository
  {
    public bool EmailExists { get; init; }

    public bool UsernameExists { get; init; }

    public AuthUser? User { get; init; }

    public NewUser? LastAddedUser { get; private set; }

    public Guid? LastChangedPasswordUserId { get; private set; }

    public string? LastChangedPasswordHash { get; private set; }

    public DateTime? LastChangedPasswordAtUtc { get; private set; }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken) => Task.FromResult(EmailExists);

    public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken) => Task.FromResult(UsernameExists);

    public Task<AuthUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken) =>
      Task.FromResult(User?.Id == userId ? User : null);

    public Task<AuthUser?> FindByEmailAsync(string email, CancellationToken cancellationToken) => Task.FromResult(User);

    public Task<AuthUser> AddAsync(NewUser user, CancellationToken cancellationToken)
    {
      LastAddedUser = user;

      return Task.FromResult(new AuthUser(Guid.NewGuid(), user.Email, user.Username, user.PasswordHash, true, false));
    }

    public Task ChangePasswordAsync(Guid userId, string passwordHash, DateTime changedAtUtc, CancellationToken cancellationToken)
    {
      LastChangedPasswordUserId = userId;
      LastChangedPasswordHash = passwordHash;
      LastChangedPasswordAtUtc = changedAtUtc;
      return Task.CompletedTask;
    }
  }

  private sealed class FakeRefreshTokenRepository : IRefreshTokenRepository
  {
    public AuthRefreshToken? LastAddedRefreshToken { get; private set; }

    public StoredRefreshToken? StoredRefreshToken { get; init; }

    public string? LastLookupTokenHash { get; private set; }

    public Guid? LastRotatedTokenId { get; private set; }

    public AuthRefreshToken? LastRotatedToken { get; private set; }

    public DateTime? LastRevokedAtUtc { get; private set; }

    public string? LastRevokedTokenHash { get; private set; }

    public Task AddAsync(AuthRefreshToken refreshToken, CancellationToken cancellationToken)
    {
      LastAddedRefreshToken = refreshToken;
      return Task.CompletedTask;
    }

    public Task<StoredRefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
      LastLookupTokenHash = tokenHash;
      return Task.FromResult(StoredRefreshToken);
    }

    public Task RotateAsync(Guid currentTokenId, AuthRefreshToken replacementToken, DateTime revokedAtUtc, CancellationToken cancellationToken)
    {
      LastRotatedTokenId = currentTokenId;
      LastRotatedToken = replacementToken;
      LastRevokedAtUtc = revokedAtUtc;
      return Task.CompletedTask;
    }

    public Task RevokeByTokenHashAsync(string tokenHash, DateTime revokedAtUtc, CancellationToken cancellationToken)
    {
      LastRevokedTokenHash = tokenHash;
      LastRevokedAtUtc = revokedAtUtc;
      return Task.CompletedTask;
    }
  }

  private sealed class FakePasswordHasher : IPasswordHasher
  {
    public string Hash(string password) => $"hashed::{password}";

    public bool Verify(string password, string passwordHash) => passwordHash == Hash(password);
  }

  private sealed class FakeAccessTokenService : IAccessTokenService
  {
    public GeneratedAccessToken GenerateToken(AuthenticatedUser user) =>
      new("access-token-raw", DateTime.UtcNow.AddMinutes(15));
  }

  private sealed class FakeRefreshTokenService : IRefreshTokenService
  {
    public GeneratedRefreshToken GenerateToken() =>
      new("refresh-token-raw", "refresh-token-hash", DateTime.UtcNow.AddDays(7));

    public string HashToken(string token) => $"hashed::{token}";
  }
}
