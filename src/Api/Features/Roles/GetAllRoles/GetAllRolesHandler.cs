using Microsoft.EntityFrameworkCore;

using SaaS.Api.Features.Roles.CreateRole;
using SaaS.Api.Infrastructure.Database;

namespace SaaS.Api.Features.Roles.GetAllRoles;

public static class GetAllRolesHandler
{
    public static async Task<IResult> HandleAsync(
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        var roles = await db.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        return Results.Ok(roles.Select(CreateRoleHandler.ToResponse));
    }
}
