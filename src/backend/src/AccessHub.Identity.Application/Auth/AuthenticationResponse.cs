namespace AccessHub.Identity.Application.Auth;

public sealed record AuthenticationResponse(AuthenticatedUser User, AuthenticationTokens Tokens);
