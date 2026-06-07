using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Users.DeleteUser;

public static class DeleteUserEndpoint
{
    public static IEndpointRouteBuilder MapDeleteUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/users/{displayId}", DeleteUserHandler.HandleAsync)
            .WithName("DeleteUser")
            .WithTags("Users")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.TenantAdmin, RoleNames.SuperAdmin));

        return app;
    }
}
