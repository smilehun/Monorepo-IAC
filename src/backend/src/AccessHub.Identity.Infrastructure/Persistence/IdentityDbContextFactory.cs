using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AccessHub.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
  public IdentityDbContext CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
    var connectionString = Environment.GetEnvironmentVariable("DATABASE__CONNECTIONSTRING")
      ?? "Host=localhost;Port=5432;Database=accesshub_identity;Username=postgres;Password=postgres";

    optionsBuilder.UseNpgsql(connectionString);

    return new IdentityDbContext(optionsBuilder.Options);
  }
}

public static class IdentityDbContextFactoryProgram
{
  public static void Main(string[] args)
  {
  }
}
