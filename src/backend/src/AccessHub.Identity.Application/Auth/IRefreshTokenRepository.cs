namespace AccessHub.Identity.Application.Auth;

public interface IRefreshTokenRepository
{
  Task AddAsync(AuthRefreshToken refreshToken, CancellationToken cancellationToken);

  Task<StoredRefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

  Task RotateAsync(Guid currentTokenId, AuthRefreshToken replacementToken, DateTime revokedAtUtc, CancellationToken cancellationToken);

  Task RevokeByTokenHashAsync(string tokenHash, DateTime revokedAtUtc, CancellationToken cancellationToken);
}
