using AccessHub.Identity.Application.Auth;
using AccessHub.Identity.Domain.Entities;
using AccessHub.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Identity.Infrastructure.Persistence.Repositories;

public sealed class AuthUserRepository(IdentityDbContext dbContext) : IAuthUserRepository
{
  public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
  {
    return dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken);
  }

  public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken)
  {
    return dbContext.Users.AnyAsync(user => user.Username == username, cancellationToken);
  }

  public async Task<AuthUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    return await dbContext.Users
      .AsNoTracking()
      .Where(user => user.Id == userId)
      .Select(user => new AuthUser(
        user.Id,
        user.Email,
        user.Username,
        user.PasswordHash,
        user.IsEnabled,
        user.IsLocked))
      .SingleOrDefaultAsync(cancellationToken);
  }

  public async Task<AuthUser?> FindByEmailAsync(string email, CancellationToken cancellationToken)
  {
    return await dbContext.Users
      .AsNoTracking()
      .Where(user => user.Email == email)
      .Select(user => new AuthUser(
        user.Id,
        user.Email,
        user.Username,
        user.PasswordHash,
        user.IsEnabled,
        user.IsLocked))
      .SingleOrDefaultAsync(cancellationToken);
  }

  public async Task<AuthUser> AddAsync(NewUser user, CancellationToken cancellationToken)
  {
    var now = DateTime.UtcNow;

    var entity = new User
    {
      Id = Guid.NewGuid(),
      Email = user.Email,
      Username = user.Username,
      PasswordHash = user.PasswordHash,
      IsEnabled = true,
      IsLocked = false,
      CreatedAtUtc = now,
      UpdatedAtUtc = now
    };

    dbContext.Users.Add(entity);
    await dbContext.SaveChangesAsync(cancellationToken);

    return new AuthUser(
      entity.Id,
      entity.Email,
      entity.Username,
      entity.PasswordHash,
      entity.IsEnabled,
      entity.IsLocked);
  }

  public async Task ChangePasswordAsync(Guid userId, string passwordHash, DateTime changedAtUtc, CancellationToken cancellationToken)
  {
    var user = await dbContext.Users.SingleAsync(entity => entity.Id == userId, cancellationToken);
    var refreshTokens = await dbContext.RefreshTokens
      .Where(token => token.UserId == userId && token.RevokedAtUtc == null)
      .ToListAsync(cancellationToken);

    user.PasswordHash = passwordHash;
    user.UpdatedAtUtc = changedAtUtc;

    foreach (var refreshToken in refreshTokens)
    {
      refreshToken.RevokedAtUtc = changedAtUtc;
    }

    await dbContext.SaveChangesAsync(cancellationToken);
  }
}
