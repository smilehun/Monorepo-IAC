namespace AccessHub.Identity.Application.Auth;

public interface IRefreshTokenService
{
  GeneratedRefreshToken GenerateToken();

  string HashToken(string token);
}
