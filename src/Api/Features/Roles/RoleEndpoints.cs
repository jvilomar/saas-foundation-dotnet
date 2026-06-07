using SaaS.Api.Features.Roles.CreateRole;
using SaaS.Api.Features.Roles.DeleteRole;
using SaaS.Api.Features.Roles.GetAllRoles;
using SaaS.Api.Features.Roles.GetRoleById;
using SaaS.Api.Features.Roles.UpdateRole;

namespace SaaS.Api.Features.Roles;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateRoleEndpoint();
        app.MapGetRoleByIdEndpoint();
        app.MapGetAllRolesEndpoint();
        app.MapUpdateRoleEndpoint();
        app.MapDeleteRoleEndpoint();

        return app;
    }
}
