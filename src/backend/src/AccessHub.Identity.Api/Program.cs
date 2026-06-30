using System.Text;
using AccessHub.Identity.Api.Authorization;
using AccessHub.Identity.Api.Configuration;
using AccessHub.Identity.Api.Endpoints;
using AccessHub.Identity.Application;
using AccessHub.Identity.Application.Authorization;
using AccessHub.Identity.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
  .ReadFrom.Configuration(context.Configuration)
  .ReadFrom.Services(services)
  .Enrich.FromLogContext()
  .WriteTo.Console());

builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

builder.Services
  .AddOptions<DatabaseOptions>()
  .BindConfiguration(DatabaseOptions.SectionName)
  .ValidateDataAnnotations()
  .ValidateOnStart();

builder.Services
  .AddOptions<JwtOptions>()
  .BindConfiguration(JwtOptions.SectionName)
  .ValidateDataAnnotations()
  .Validate(options => Encoding.UTF8.GetByteCount(options.SigningKey) >= 32, "Jwt:SigningKey must be at least 32 bytes long.")
  .ValidateOnStart();

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer();

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy(
    AuthorizationPolicies.ViewCurrentUser,
    policy => policy.Requirements.Add(new PermissionRequirement(AuthorizationPermissions.ViewCurrentUser)));
});

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services
  .AddApplication()
  .AddInfrastructure(builder.Configuration);

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
  .Configure<IOptions<JwtOptions>>((options, jwtOptionsAccessor) =>
  {
    var jwtOptions = jwtOptionsAccessor.Value;

    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidIssuer = jwtOptions.Issuer,
      ValidateAudience = true,
      ValidAudience = jwtOptions.Audience,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
      ValidateLifetime = true,
      ClockSkew = TimeSpan.Zero,
      NameClaimType = "unique_name"
    };
  });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
  service = "AccessHub.Identity",
  status = "ok"
}));

app.MapHealthChecks("/health");
app.MapAuthEndpoints();

app.Run();

public partial class Program
{
}
