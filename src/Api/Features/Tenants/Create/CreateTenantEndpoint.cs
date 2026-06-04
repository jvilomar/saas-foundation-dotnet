using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Tenants.Create;

public static class CreateTenantEndpoint
{
    public static IEndpointRouteBuilder MapCreateTenantEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tenants", CreateTenantHandler.HandleAsync)
            .WithName("CreateTenant")
            .WithTags("Tenants")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.SuperAdmin));

        return app;
    }
}
