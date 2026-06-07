using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Roles.DeleteRole;

public static class DeleteRoleEndpoint
{
    public static IEndpointRouteBuilder MapDeleteRoleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/roles/{id:guid}", DeleteRoleHandler.HandleAsync)
            .WithName("DeleteRole")
            .WithTags("Roles")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.SuperAdmin));

        return app;
    }
}
