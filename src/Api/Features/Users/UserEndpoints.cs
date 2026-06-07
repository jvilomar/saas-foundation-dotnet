using SaaS.Api.Features.Users.CreateUser;
using SaaS.Api.Features.Users.DeleteUser;
using SaaS.Api.Features.Users.GetAllUsers;
using SaaS.Api.Features.Users.GetUserById;
using SaaS.Api.Features.Users.UpdateUser;

namespace SaaS.Api.Features.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateUserEndpoint();
        app.MapGetUserByIdEndpoint();
        app.MapGetAllUsersEndpoint();
        app.MapUpdateUserEndpoint();
        app.MapDeleteUserEndpoint();

        return app;
    }
}
