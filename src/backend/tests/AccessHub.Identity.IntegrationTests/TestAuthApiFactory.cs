using AccessHub.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AccessHub.Identity.IntegrationTests;

public sealed class TestAuthApiFactory : WebApplicationFactory<global::Program>
{
  private readonly string databaseName = $"accesshub-identity-tests-{Guid.NewGuid()}";

  public TestAuthApiFactory()
  {
    Environment.SetEnvironmentVariable("Database__ConnectionString", "Host=localhost;Database=accesshub_identity_tests;Username=test;Password=test");
    Environment.SetEnvironmentVariable("Jwt__Issuer", "AccessHub.Identity.Tests");
    Environment.SetEnvironmentVariable("Jwt__Audience", "AccessHub.Identity.Tests.Client");
    Environment.SetEnvironmentVariable("Jwt__SigningKey", "integration-tests-signing-key-1234567890");
    Environment.SetEnvironmentVariable("Jwt__AccessTokenLifetimeMinutes", "15");
    Environment.SetEnvironmentVariable("Jwt__RefreshTokenLifetimeDays", "7");
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Development");

    builder.ConfigureServices(services =>
    {
      services.RemoveAll(typeof(DbContextOptions<IdentityDbContext>));
      services.RemoveAll(typeof(IdentityDbContext));
      services.RemoveAll(typeof(IDbContextOptionsConfiguration<IdentityDbContext>));

      services.AddDbContext<IdentityDbContext>(options =>
        options.UseInMemoryDatabase(databaseName));
    });
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      Environment.SetEnvironmentVariable("Database__ConnectionString", null);
      Environment.SetEnvironmentVariable("Jwt__Issuer", null);
      Environment.SetEnvironmentVariable("Jwt__Audience", null);
      Environment.SetEnvironmentVariable("Jwt__SigningKey", null);
      Environment.SetEnvironmentVariable("Jwt__AccessTokenLifetimeMinutes", null);
      Environment.SetEnvironmentVariable("Jwt__RefreshTokenLifetimeDays", null);
    }

    base.Dispose(disposing);
  }
}
