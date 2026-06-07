using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Roles.GetRoleById;

public static class GetRoleByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetRoleByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/roles/{id:guid}", GetRoleByIdHandler.HandleAsync)
            .WithName("GetRoleById")
            .WithTags("Roles")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.SuperAdmin, RoleNames.TenantAdmin));

        return app;
    }
}
