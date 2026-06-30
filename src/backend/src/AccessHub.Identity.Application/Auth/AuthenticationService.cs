namespace AccessHub.Identity.Application.Auth;

public sealed class AuthenticationService(
  IAuthUserRepository userRepository,
  IRefreshTokenRepository refreshTokenRepository,
  IPasswordHasher passwordHasher,
  IAccessTokenService accessTokenService,
  IRefreshTokenService refreshTokenService) : IAuthenticationService
{
  public async Task<AuthResult> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken)
  {
    var validationErrors = AuthInputValidator.Validate(command);

    if (validationErrors.Count > 0)
    {
      return AuthResult.Validation(validationErrors);
    }

    var normalizedEmail = AuthInputValidator.NormalizeEmail(command.Email);
    var normalizedUsername = AuthInputValidator.NormalizeUsername(command.Username);

    if (await userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
    {
      return AuthResult.DuplicateEmail();
    }

    if (await userRepository.UsernameExistsAsync(normalizedUsername, cancellationToken))
    {
      return AuthResult.DuplicateUsername();
    }

    var passwordHash = passwordHasher.Hash(command.Password);

    var user = await userRepository.AddAsync(new NewUser(normalizedEmail, normalizedUsername, passwordHash), cancellationToken);

    return await IssueAuthenticationAsync(user, cancellationToken);
  }

  public async Task<AuthResult> LoginAsync(LoginUserCommand command, CancellationToken cancellationToken)
  {
    var validationErrors = AuthInputValidator.Validate(command);

    if (validationErrors.Count > 0)
    {
      return AuthResult.Validation(validationErrors);
    }

    var normalizedEmail = AuthInputValidator.NormalizeEmail(command.Email);
    var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

    if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
    {
      return AuthResult.InvalidCredentials();
    }

    if (!user.IsEnabled)
    {
      return AuthResult.UserDisabled();
    }

    if (user.IsLocked)
    {
      return AuthResult.UserLocked();
    }

    return await IssueAuthenticationAsync(user, cancellationToken);
  }

  public async Task<AuthResult> RefreshAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
  {
    var validationErrors = AuthInputValidator.Validate(command);

    if (validationErrors.Count > 0)
    {
      return AuthResult.Validation(validationErrors);
    }

    var tokenHash = refreshTokenService.HashToken(command.RefreshToken);
    var storedToken = await refreshTokenRepository.FindByTokenHashAsync(tokenHash, cancellationToken);

    if (storedToken is null || storedToken.ExpiresAtUtc <= DateTime.UtcNow || storedToken.RevokedAtUtc is not null)
    {
      return AuthResult.InvalidRefreshToken();
    }

    if (!storedToken.IsEnabled)
    {
      return AuthResult.UserDisabled();
    }

    if (storedToken.IsLocked)
    {
      return AuthResult.UserLocked();
    }

    var user = new AuthUser(
      storedToken.UserId,
      storedToken.Email,
      storedToken.Username,
      string.Empty,
      storedToken.IsEnabled,
      storedToken.IsLocked);

    return await RotateRefreshTokenAsync(storedToken.Id, user, cancellationToken);
  }

  public async Task<LogoutResult> LogoutAsync(LogoutCommand command, CancellationToken cancellationToken)
  {
    var validationErrors = AuthInputValidator.Validate(command);

    if (validationErrors.Count > 0)
    {
      return LogoutResult.Validation(validationErrors);
    }

    var tokenHash = refreshTokenService.HashToken(command.RefreshToken);

    await refreshTokenRepository.RevokeByTokenHashAsync(tokenHash, DateTime.UtcNow, cancellationToken);

    return LogoutResult.Success();
  }

  public async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken)
  {
    var validationErrors = AuthInputValidator.Validate(command);

    if (validationErrors.Count > 0)
    {
      return ChangePasswordResult.Validation(validationErrors);
    }

    var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);

    if (user is null || !passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
    {
      return ChangePasswordResult.InvalidCredentials();
    }

    if (!user.IsEnabled)
    {
      return ChangePasswordResult.UserDisabled();
    }

    if (user.IsLocked)
    {
      return ChangePasswordResult.UserLocked();
    }

    var now = DateTime.UtcNow;
    var newPasswordHash = passwordHasher.Hash(command.NewPassword);

    await userRepository.ChangePasswordAsync(user.Id, newPasswordHash, now, cancellationToken);

    return ChangePasswordResult.Success();
  }

  private async Task<AuthResult> IssueAuthenticationAsync(AuthUser user, CancellationToken cancellationToken)
  {
    var authenticatedUser = new AuthenticatedUser(user.Id, user.Email, user.Username);
    var accessToken = accessTokenService.GenerateToken(authenticatedUser);
    var refreshToken = refreshTokenService.GenerateToken();

    await refreshTokenRepository.AddAsync(
      new AuthRefreshToken(Guid.NewGuid(), user.Id, refreshToken.TokenHash, DateTime.UtcNow, refreshToken.ExpiresAtUtc),
      cancellationToken);

    return AuthResult.Success(
      new AuthenticationResponse(
        authenticatedUser,
        new AuthenticationTokens(
          accessToken.Token,
          accessToken.ExpiresAtUtc,
          refreshToken.Token,
          refreshToken.ExpiresAtUtc)));
  }

  private async Task<AuthResult> RotateRefreshTokenAsync(Guid currentTokenId, AuthUser user, CancellationToken cancellationToken)
  {
    var authenticatedUser = new AuthenticatedUser(user.Id, user.Email, user.Username);
    var accessToken = accessTokenService.GenerateToken(authenticatedUser);
    var refreshToken = refreshTokenService.GenerateToken();
    var now = DateTime.UtcNow;
    var replacementToken = new AuthRefreshToken(Guid.NewGuid(), user.Id, refreshToken.TokenHash, now, refreshToken.ExpiresAtUtc);

    await refreshTokenRepository.RotateAsync(currentTokenId, replacementToken, now, cancellationToken);

    return AuthResult.Success(
      new AuthenticationResponse(
        authenticatedUser,
        new AuthenticationTokens(
          accessToken.Token,
          accessToken.ExpiresAtUtc,
          refreshToken.Token,
          refreshToken.ExpiresAtUtc)));
  }
}
