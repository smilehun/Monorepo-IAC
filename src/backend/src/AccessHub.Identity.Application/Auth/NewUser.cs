namespace AccessHub.Identity.Application.Auth;

public sealed record NewUser(string Email, string Username, string PasswordHash);
