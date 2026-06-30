using AccessHub.Identity.Application.Auth;
using AccessHub.Identity.Application.Authorization;
using AccessHub.Identity.Infrastructure.Auth;
using AccessHub.Identity.Infrastructure.Persistence;
using AccessHub.Identity.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccessHub.Identity.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration["Database:ConnectionString"];

    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new InvalidOperationException("Database:ConnectionString is required.");
    }

    services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(connectionString));

    services.AddScoped<IAuthUserRepository, AuthUserRepository>();
    services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    services.AddScoped<IUserPermissionReader, UserPermissionReader>();
    services.AddSingleton<IPasswordHasher, PasswordHasher>();
    services.AddSingleton<IAccessTokenService, AccessTokenService>();
    services.AddSingleton<IRefreshTokenService, RefreshTokenService>();

    services.AddHealthChecks()
      .AddDbContextCheck<IdentityDbContext>("database");

    return services;
  }
}
