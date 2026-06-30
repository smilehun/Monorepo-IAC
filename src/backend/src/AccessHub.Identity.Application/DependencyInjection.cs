using AccessHub.Identity.Application.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace AccessHub.Identity.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<IAuthenticationService, AuthenticationService>();

    return services;
  }
}
