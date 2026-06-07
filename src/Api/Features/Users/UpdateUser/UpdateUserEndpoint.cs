using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Users.UpdateUser;

public static class UpdateUserEndpoint
{
    public static IEndpointRouteBuilder MapUpdateUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/{displayId}", UpdateUserHandler.HandleAsync)
            .WithName("UpdateUser")
            .WithTags("Users")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.TenantAdmin, RoleNames.SuperAdmin));

        return app;
    }
}
