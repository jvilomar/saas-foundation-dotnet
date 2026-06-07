using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Roles.CreateRole;

public static class CreateRoleEndpoint
{
    public static IEndpointRouteBuilder MapCreateRoleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/roles", CreateRoleHandler.HandleAsync)
            .WithName("CreateRole")
            .WithTags("Roles")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.SuperAdmin));

        return app;
    }
}
