namespace SaaS.Api.Features.Users.CreateUser;

public sealed record CreateUserRequest(string Email, string Password, Guid RoleId);
