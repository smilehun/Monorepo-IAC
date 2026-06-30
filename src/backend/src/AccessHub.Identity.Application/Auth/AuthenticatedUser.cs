namespace AccessHub.Identity.Application.Auth;

public sealed record AuthenticatedUser(Guid Id, string Email, string Username);
