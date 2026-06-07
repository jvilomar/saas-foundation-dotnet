using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Users.GetUserById;

public static class GetUserByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetUserByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{displayId}", GetUserByIdHandler.HandleAsync)
            .WithName("GetUserById")
            .WithTags("Users")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.TenantAdmin, RoleNames.SuperAdmin));

        return app;
    }
}
