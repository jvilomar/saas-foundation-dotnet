using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Users.GetAllUsers;

public static class GetAllUsersEndpoint
{
    public static IEndpointRouteBuilder MapGetAllUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", GetAllUsersHandler.HandleAsync)
            .WithName("GetAllUsers")
            .WithTags("Users")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.TenantAdmin, RoleNames.SuperAdmin));

        return app;
    }
}
