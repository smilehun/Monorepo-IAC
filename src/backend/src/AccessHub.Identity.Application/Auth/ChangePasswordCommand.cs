namespace AccessHub.Identity.Application.Auth;

public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);
