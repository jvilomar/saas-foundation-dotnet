namespace SaaS.Api.Features.Users;

public sealed record UserResponse(
    string DisplayId,
    string Email,
    Guid TenantId,
    Guid RoleId,
    string RoleName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastModifiedAt);
