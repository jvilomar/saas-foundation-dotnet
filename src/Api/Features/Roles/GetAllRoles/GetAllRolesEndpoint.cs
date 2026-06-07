using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Roles.GetAllRoles;

public static class GetAllRolesEndpoint
{
    public static IEndpointRouteBuilder MapGetAllRolesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/roles", GetAllRolesHandler.HandleAsync)
            .WithName("GetAllRoles")
            .WithTags("Roles")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.SuperAdmin, RoleNames.TenantAdmin));

        return app;
    }
}
