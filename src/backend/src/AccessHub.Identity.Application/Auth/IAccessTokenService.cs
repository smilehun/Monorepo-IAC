namespace AccessHub.Identity.Application.Auth;

public interface IAccessTokenService
{
  GeneratedAccessToken GenerateToken(AuthenticatedUser user);
}
