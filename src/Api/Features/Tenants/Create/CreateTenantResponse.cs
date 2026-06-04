namespace SaaS.Api.Features.Tenants.Create;

public sealed record CreateTenantResponse(
    string DisplayId,
    string Slug,
    string Name,
    DateTime CreatedAt);
