namespace AccessHub.Identity.Application.Auth;

public sealed record RegisterUserCommand(string Email, string Username, string Password);
