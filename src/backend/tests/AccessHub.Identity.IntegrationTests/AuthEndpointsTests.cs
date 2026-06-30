using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AccessHub.Identity.Application.Authorization;
using AccessHub.Identity.Domain.Entities;
using AccessHub.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AccessHub.Identity.IntegrationTests;

public sealed class AuthEndpointsTests
{
  [Fact]
  public async Task Register_PersistsUser_AndReturnsTokens()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/auth/register", new
    {
      Email = "User@example.com",
      Username = "user1",
      Password = "Password123"
    });

    response.EnsureSuccessStatusCode();

    var payload = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

    Assert.NotNull(payload);
    Assert.False(string.IsNullOrWhiteSpace(payload.Tokens.AccessToken));
    Assert.False(string.IsNullOrWhiteSpace(payload.Tokens.RefreshToken));
    Assert.Equal("user@example.com", payload.User.Email);

    using var scope = factory.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    var user = await dbContext.Users.SingleAsync();
    var refreshToken = await dbContext.RefreshTokens.SingleAsync();

    Assert.Equal("user@example.com", user.Email);
    Assert.NotEqual("Password123", user.PasswordHash);
    Assert.DoesNotContain(payload.Tokens.RefreshToken, refreshToken.TokenHash, StringComparison.Ordinal);
    Assert.False(string.IsNullOrWhiteSpace(refreshToken.TokenHash));
  }

  [Fact]
  public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    await client.PostAsJsonAsync("/api/auth/register", new
    {
      Email = "user@example.com",
      Username = "user2",
      Password = "Password123"
    });

    var response = await client.PostAsJsonAsync("/api/auth/login", new
    {
      Email = "user@example.com",
      Password = "wrong-password"
    });

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task Login_ReturnsTokens_AndIssuedJwtCanAccessProtectedEndpoint()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    await client.PostAsJsonAsync("/api/auth/register", new
    {
      Email = "user3@example.com",
      Username = "user3",
      Password = "Password123"
    });

    var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
    {
      Email = "user3@example.com",
      Password = "Password123"
    });

    loginResponse.EnsureSuccessStatusCode();

    var payload = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
    Assert.NotNull(payload);

    await GrantViewCurrentUserPermissionAsync(factory, payload.User.Id);

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload.Tokens.AccessToken);

    var meResponse = await client.GetAsync("/api/auth/me");
    meResponse.EnsureSuccessStatusCode();

    var me = await meResponse.Content.ReadFromJsonAsync<CurrentUserDto>();

    Assert.NotNull(me);
    Assert.Equal("user3@example.com", me.Email);
    Assert.Equal("user3", me.Username);
  }

  [Fact]
  public async Task GetCurrentUser_ReturnsUnauthorized_WhenBearerTokenIsMissing()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    var response = await client.GetAsync("/api/auth/me");

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task GetCurrentUser_ReturnsForbidden_WhenPermissionIsMissing()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
    {
      Email = "user-no-permission@example.com",
      Username = "userNoPermission",
      Password = "Password123"
    });

    registerResponse.EnsureSuccessStatusCode();

    var payload = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
    Assert.NotNull(payload);

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload.Tokens.AccessToken);

    var response = await client.GetAsync("/api/auth/me");

    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
  }

  [Fact]
  public async Task Refresh_ReturnsNewTokens_RevokesOldToken_AndRejectsReplay()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
    {
      Email = "user4@example.com",
      Username = "user4",
      Password = "Password123"
    });

    registerResponse.EnsureSuccessStatusCode();

    var registerPayload = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
    Assert.NotNull(registerPayload);

    var originalRefreshToken = registerPayload.Tokens.RefreshToken;

    var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
    {
      RefreshToken = originalRefreshToken
    });

    refreshResponse.EnsureSuccessStatusCode();

    var refreshedPayload = await refreshResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
    Assert.NotNull(refreshedPayload);
    Assert.NotEqual(originalRefreshToken, refreshedPayload.Tokens.RefreshToken);
    Assert.NotEqual(registerPayload.Tokens.AccessToken, refreshedPayload.Tokens.AccessToken);

    using (var scope = factory.Services.CreateScope())
    {
      var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
      var refreshTokens = await dbContext.RefreshTokens.OrderBy(token => token.CreatedAtUtc).ToListAsync();

      Assert.Equal(2, refreshTokens.Count);
      Assert.NotNull(refreshTokens[0].RevokedAtUtc);
      Assert.NotNull(refreshTokens[0].ReplacedByTokenId);
      Assert.Equal(refreshTokens[1].Id, refreshTokens[0].ReplacedByTokenId);
      Assert.Null(refreshTokens[1].RevokedAtUtc);
      Assert.DoesNotContain(refreshedPayload.Tokens.RefreshToken, refreshTokens[1].TokenHash, StringComparison.Ordinal);
    }

    var replayResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
    {
      RefreshToken = originalRefreshToken
    });

    Assert.Equal(HttpStatusCode.Unauthorized, replayResponse.StatusCode);

    await GrantViewCurrentUserPermissionAsync(factory, registerPayload.User.Id);

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshedPayload.Tokens.AccessToken);

    var meResponse = await client.GetAsync("/api/auth/me");
    meResponse.EnsureSuccessStatusCode();

    var me = await meResponse.Content.ReadFromJsonAsync<CurrentUserDto>();
    Assert.NotNull(me);
    Assert.Equal("user4@example.com", me.Email);
    Assert.Equal("user4", me.Username);
  }

  [Fact]
  public async Task Logout_RevokesRefreshToken_IsIdempotent_AndBlocksFutureRefresh()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
    {
      Email = "user5@example.com",
      Username = "user5",
      Password = "Password123"
    });

    registerResponse.EnsureSuccessStatusCode();

    var registerPayload = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
    Assert.NotNull(registerPayload);

    var refreshToken = registerPayload.Tokens.RefreshToken;

    var logoutResponse = await client.PostAsJsonAsync("/api/auth/logout", new
    {
      RefreshToken = refreshToken
    });

    Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

    using (var scope = factory.Services.CreateScope())
    {
      var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
      var refreshTokens = await dbContext.RefreshTokens.ToListAsync();

      Assert.Single(refreshTokens);
      Assert.NotNull(refreshTokens[0].RevokedAtUtc);
      Assert.Null(refreshTokens[0].ReplacedByTokenId);
    }

    var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
    {
      RefreshToken = refreshToken
    });

    Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);

    var repeatedLogoutResponse = await client.PostAsJsonAsync("/api/auth/logout", new
    {
      RefreshToken = refreshToken
    });

    Assert.Equal(HttpStatusCode.NoContent, repeatedLogoutResponse.StatusCode);
  }

  [Fact]
  public async Task ChangePassword_ReturnsNoContent_RevokesExistingRefreshToken_AndRequiresNewPasswordForLogin()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
    {
      Email = "user6@example.com",
      Username = "user6",
      Password = "Password123"
    });

    registerResponse.EnsureSuccessStatusCode();

    var registerPayload = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
    Assert.NotNull(registerPayload);

    string oldPasswordHash;
    var oldRefreshToken = registerPayload.Tokens.RefreshToken;

    using (var scope = factory.Services.CreateScope())
    {
      var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
      var userBeforeChange = await dbContext.Users.SingleAsync(user => user.Email == "user6@example.com");
      oldPasswordHash = userBeforeChange.PasswordHash;
    }

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", registerPayload.Tokens.AccessToken);

    var changePasswordResponse = await client.PostAsJsonAsync("/api/auth/change-password", new
    {
      CurrentPassword = "Password123",
      NewPassword = "Password456"
    });

    Assert.Equal(HttpStatusCode.NoContent, changePasswordResponse.StatusCode);

    using (var scope = factory.Services.CreateScope())
    {
      var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
      var userAfterChange = await dbContext.Users.SingleAsync(user => user.Email == "user6@example.com");
      var refreshToken = await dbContext.RefreshTokens.SingleAsync();

      Assert.NotEqual(oldPasswordHash, userAfterChange.PasswordHash);
      Assert.NotNull(refreshToken.RevokedAtUtc);
    }

    client.DefaultRequestHeaders.Authorization = null;

    var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new
    {
      RefreshToken = oldRefreshToken
    });

    Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);

    var oldPasswordLoginResponse = await client.PostAsJsonAsync("/api/auth/login", new
    {
      Email = "user6@example.com",
      Password = "Password123"
    });

    Assert.Equal(HttpStatusCode.Unauthorized, oldPasswordLoginResponse.StatusCode);

    var newPasswordLoginResponse = await client.PostAsJsonAsync("/api/auth/login", new
    {
      Email = "user6@example.com",
      Password = "Password456"
    });

    newPasswordLoginResponse.EnsureSuccessStatusCode();
  }

  [Fact]
  public async Task ChangePassword_ReturnsUnauthorized_WhenBearerTokenIsMissing()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/auth/change-password", new
    {
      CurrentPassword = "Password123",
      NewPassword = "Password456"
    });

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task ChangePassword_ReturnsBadRequest_WhenInputIsMalformed()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
    {
      Email = "user7@example.com",
      Username = "user7",
      Password = "Password123"
    });

    registerResponse.EnsureSuccessStatusCode();

    var registerPayload = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
    Assert.NotNull(registerPayload);

    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", registerPayload.Tokens.AccessToken);

    var response = await client.PostAsJsonAsync("/api/auth/change-password", new
    {
      CurrentPassword = "",
      NewPassword = "short"
    });

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task Logout_ReturnsNoContent_ForUnknownToken_AndValidationError_ForBlankToken()
  {
    await using var factory = new TestAuthApiFactory();
    using var client = factory.CreateClient();

    var unknownTokenResponse = await client.PostAsJsonAsync("/api/auth/logout", new
    {
      RefreshToken = "unknown-token"
    });

    Assert.Equal(HttpStatusCode.NoContent, unknownTokenResponse.StatusCode);

    var blankTokenResponse = await client.PostAsJsonAsync("/api/auth/logout", new
    {
      RefreshToken = ""
    });

    Assert.Equal(HttpStatusCode.BadRequest, blankTokenResponse.StatusCode);
  }

  private static async Task GrantViewCurrentUserPermissionAsync(TestAuthApiFactory factory, Guid userId)
  {
    using var scope = factory.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

    var role = new Role
    {
      Id = Guid.NewGuid(),
      Name = $"current-user-viewer-{userId:N}",
      Description = "Can view the current user."
    };

    var permission = await dbContext.Permissions.SingleOrDefaultAsync(
      existingPermission => existingPermission.Name == AuthorizationPermissions.ViewCurrentUser);

    if (permission is null)
    {
      permission = new Permission
      {
        Id = Guid.NewGuid(),
        Name = AuthorizationPermissions.ViewCurrentUser
      };

      dbContext.Permissions.Add(permission);
    }

    dbContext.Roles.Add(role);
    dbContext.RolePermissions.Add(new RolePermission
    {
      RoleId = role.Id,
      PermissionId = permission.Id
    });
    dbContext.UserRoles.Add(new UserRole
    {
      UserId = userId,
      RoleId = role.Id
    });

    await dbContext.SaveChangesAsync();
  }

  public sealed record AuthResponseDto(UserDto User, TokensDto Tokens);

  public sealed record UserDto(Guid Id, string Email, string Username);

  public sealed record TokensDto(string AccessToken, DateTime AccessTokenExpiresAtUtc, string RefreshToken, DateTime RefreshTokenExpiresAtUtc);

  public sealed record CurrentUserDto(string UserId, string Email, string Username);
}
