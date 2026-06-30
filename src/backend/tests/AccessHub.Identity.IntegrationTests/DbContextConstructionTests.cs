using AccessHub.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AccessHub.Identity.IntegrationTests;

public class DbContextConstructionTests
{
  [Fact]
  public void CanConstructIdentityDbContext()
  {
    var options = new DbContextOptionsBuilder<IdentityDbContext>()
      .UseInMemoryDatabase("identity-db-context-construction")
      .Options;

    using var dbContext = new IdentityDbContext(options);

    Assert.NotNull(dbContext);
  }
}
