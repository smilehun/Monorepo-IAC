namespace AccessHub.Identity.Application.Auth;

public interface IAuthUserRepository
{
  Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

  Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken);

  Task<AuthUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);

  Task<AuthUser?> FindByEmailAsync(string email, CancellationToken cancellationToken);

  Task<AuthUser> AddAsync(NewUser user, CancellationToken cancellationToken);

  Task ChangePasswordAsync(Guid userId, string passwordHash, DateTime changedAtUtc, CancellationToken cancellationToken);
}
