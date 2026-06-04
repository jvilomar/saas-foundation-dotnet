namespace SaaS.Api.Features.Auth.Login;

public sealed record LoginResponse(
    string Token,
    string Email,
    string Role,
    string UserDisplayId,
    string TenantDisplayId,
    string WorkspaceSlug);
