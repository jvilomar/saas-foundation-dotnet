using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Users.CreateUser;

public static class CreateUserEndpoint
{
    public static IEndpointRouteBuilder MapCreateUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", CreateUserHandler.HandleAsync)
            .WithName("CreateUser")
            .WithTags("Users")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.TenantAdmin, RoleNames.SuperAdmin));

        return app;
    }
}
