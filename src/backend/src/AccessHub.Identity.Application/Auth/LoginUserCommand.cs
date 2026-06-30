namespace AccessHub.Identity.Application.Auth;

public sealed record LoginUserCommand(string Email, string Password);
