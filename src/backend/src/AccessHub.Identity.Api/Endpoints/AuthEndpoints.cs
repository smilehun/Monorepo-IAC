using AccessHub.Identity.Application.Auth;
using ApplicationAuthenticationService = AccessHub.Identity.Application.Auth.IAuthenticationService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AccessHub.Identity.Api.Endpoints;

public static class AuthEndpoints
{
  public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
  {
    var group = endpoints.MapGroup("/api/auth");

    group.MapPost("/register", RegisterAsync)
      .AllowAnonymous();

    group.MapPost("/login", LoginAsync)
      .AllowAnonymous();

    group.MapPost("/refresh", RefreshAsync)
      .AllowAnonymous();

    group.MapPost("/logout", LogoutAsync)
      .AllowAnonymous();

    group.MapPost("/change-password", ChangePasswordAsync)
      .RequireAuthorization();

    group.MapGet("/me", GetCurrentUser)
      .RequireAuthorization();

    return endpoints;
  }

  private static async Task<IResult> RegisterAsync(
    RegisterRequest request,
    ApplicationAuthenticationService authenticationService,
    CancellationToken cancellationToken)
  {
    var result = await authenticationService.RegisterAsync(
      new RegisterUserCommand(request.Email, request.Username, request.Password),
      cancellationToken);

    return ToResult(result);
  }

  private static async Task<IResult> LoginAsync(
    LoginRequest request,
    ApplicationAuthenticationService authenticationService,
    CancellationToken cancellationToken)
  {
    var result = await authenticationService.LoginAsync(
      new LoginUserCommand(request.Email, request.Password),
      cancellationToken);

    return ToResult(result);
  }

  private static async Task<IResult> RefreshAsync(
    RefreshRequest request,
    ApplicationAuthenticationService authenticationService,
    CancellationToken cancellationToken)
  {
    var result = await authenticationService.RefreshAsync(
      new RefreshTokenCommand(request.RefreshToken),
      cancellationToken);

    return ToResult(result);
  }

  private static async Task<IResult> LogoutAsync(
    LogoutRequest request,
    ApplicationAuthenticationService authenticationService,
    CancellationToken cancellationToken)
  {
    var result = await authenticationService.LogoutAsync(
      new LogoutCommand(request.RefreshToken),
      cancellationToken);

    if (result.IsSuccess)
    {
      return Results.NoContent();
    }

    return Results.ValidationProblem(result.ValidationErrors!);
  }

  [Authorize]
  private static async Task<IResult> ChangePasswordAsync(
    ChangePasswordRequest request,
    ClaimsPrincipal claimsPrincipal,
    ApplicationAuthenticationService authenticationService,
    CancellationToken cancellationToken)
  {
    var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
      ?? claimsPrincipal.FindFirstValue("sub");

    if (!Guid.TryParse(userId, out var parsedUserId))
    {
      return Results.Unauthorized();
    }

    var result = await authenticationService.ChangePasswordAsync(
      new ChangePasswordCommand(parsedUserId, request.CurrentPassword, request.NewPassword),
      cancellationToken);

    if (result.IsSuccess)
    {
      return Results.NoContent();
    }

    return ToResult(result);
  }

  [Authorize]
  private static IResult GetCurrentUser(ClaimsPrincipal claimsPrincipal)
  {
    var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
      ?? claimsPrincipal.FindFirstValue("sub")
      ?? string.Empty;

    var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email)
      ?? claimsPrincipal.FindFirstValue("email")
      ?? string.Empty;

    var username = claimsPrincipal.FindFirstValue(ClaimTypes.Name)
      ?? claimsPrincipal.FindFirstValue("unique_name")
      ?? claimsPrincipal.Identity?.Name
      ?? string.Empty;

    return Results.Ok(new CurrentUserResponse(userId, email, username));
  }

  private static IResult ToResult(AuthResult result)
  {
    if (result.IsSuccess)
    {
      return Results.Ok(result.Response);
    }

    return ToResult(result.Failure!);
  }

  private static IResult ToResult(ChangePasswordResult result)
  {
    return ToResult(result.Failure!);
  }

  private static IResult ToResult(AuthFailure failure)
  {
    return failure.Type switch
    {
      AuthFailureType.Validation => Results.ValidationProblem(failure.ValidationErrors!),
      AuthFailureType.DuplicateEmail => Results.Conflict(new { error = failure.Message }),
      AuthFailureType.DuplicateUsername => Results.Conflict(new { error = failure.Message }),
      AuthFailureType.InvalidCredentials => Results.Json(new { error = failure.Message }, statusCode: StatusCodes.Status401Unauthorized),
      AuthFailureType.InvalidRefreshToken => Results.Json(new { error = failure.Message }, statusCode: StatusCodes.Status401Unauthorized),
      AuthFailureType.UserDisabled => Results.Json(new { error = failure.Message }, statusCode: StatusCodes.Status403Forbidden),
      AuthFailureType.UserLocked => Results.Json(new { error = failure.Message }, statusCode: StatusCodes.Status403Forbidden),
      _ => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
    };
  }

  public sealed record RegisterRequest(string Email, string Username, string Password);

  public sealed record LoginRequest(string Email, string Password);

  public sealed record RefreshRequest(string RefreshToken);

  public sealed record LogoutRequest(string RefreshToken);

  public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

  public sealed record CurrentUserResponse(string UserId, string Email, string Username);
}
