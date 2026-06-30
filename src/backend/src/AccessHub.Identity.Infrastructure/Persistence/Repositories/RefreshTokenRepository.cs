using AccessHub.Identity.Application.Auth;
using AccessHub.Identity.Domain.Entities;
using AccessHub.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.Identity.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository(IdentityDbContext dbContext) : IRefreshTokenRepository
{
  public async Task AddAsync(AuthRefreshToken refreshToken, CancellationToken cancellationToken)
  {
    dbContext.RefreshTokens.Add(ToEntity(refreshToken));
    await dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task<StoredRefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
  {
    return await dbContext.RefreshTokens
      .AsNoTracking()
      .Where(refreshToken => refreshToken.TokenHash == tokenHash)
      .Select(refreshToken => new StoredRefreshToken(
        refreshToken.Id,
        refreshToken.UserId,
        refreshToken.User.Email,
        refreshToken.User.Username,
        refreshToken.User.IsEnabled,
        refreshToken.User.IsLocked,
        refreshToken.ExpiresAtUtc,
        refreshToken.RevokedAtUtc))
      .SingleOrDefaultAsync(cancellationToken);
  }

  public async Task RotateAsync(Guid currentTokenId, AuthRefreshToken replacementToken, DateTime revokedAtUtc, CancellationToken cancellationToken)
  {
    var currentToken = await dbContext.RefreshTokens.SingleAsync(refreshToken => refreshToken.Id == currentTokenId, cancellationToken);

    currentToken.RevokedAtUtc = revokedAtUtc;
    currentToken.ReplacedByTokenId = replacementToken.Id;

    dbContext.RefreshTokens.Add(ToEntity(replacementToken));

    await dbContext.SaveChangesAsync(cancellationToken);
  }

  public async Task RevokeByTokenHashAsync(string tokenHash, DateTime revokedAtUtc, CancellationToken cancellationToken)
  {
    var refreshToken = await dbContext.RefreshTokens
      .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

    if (refreshToken is null || refreshToken.RevokedAtUtc is not null)
    {
      return;
    }

    refreshToken.RevokedAtUtc = revokedAtUtc;

    await dbContext.SaveChangesAsync(cancellationToken);
  }

  private static RefreshToken ToEntity(AuthRefreshToken refreshToken)
  {
    return new RefreshToken
    {
      Id = refreshToken.Id,
      UserId = refreshToken.UserId,
      TokenHash = refreshToken.TokenHash,
      CreatedAtUtc = refreshToken.CreatedAtUtc,
      ExpiresAtUtc = refreshToken.ExpiresAtUtc
    };
  }
}
