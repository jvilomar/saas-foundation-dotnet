using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Roles.UpdateRole;

public static class UpdateRoleEndpoint
{
    public static IEndpointRouteBuilder MapUpdateRoleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/roles/{id:guid}", UpdateRoleHandler.HandleAsync)
            .WithName("UpdateRole")
            .WithTags("Roles")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.SuperAdmin));

        return app;
    }
}
