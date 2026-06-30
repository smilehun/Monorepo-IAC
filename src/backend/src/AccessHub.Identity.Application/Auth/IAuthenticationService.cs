namespace AccessHub.Identity.Application.Auth;

public interface IAuthenticationService
{
  Task<AuthResult> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken);

  Task<AuthResult> LoginAsync(LoginUserCommand command, CancellationToken cancellationToken);

  Task<AuthResult> RefreshAsync(RefreshTokenCommand command, CancellationToken cancellationToken);

  Task<LogoutResult> LogoutAsync(LogoutCommand command, CancellationToken cancellationToken);

  Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken);
}
