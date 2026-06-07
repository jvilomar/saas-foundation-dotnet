namespace SaaS.Api.Features.Users.UpdateUser;

public sealed record UpdateUserRequest(string Email, Guid RoleId);
