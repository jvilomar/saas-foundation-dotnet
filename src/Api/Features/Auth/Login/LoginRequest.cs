namespace SaaS.Api.Features.Auth.Login;

public sealed record LoginRequest(string WorkspaceSlug, string Email, string Password);
