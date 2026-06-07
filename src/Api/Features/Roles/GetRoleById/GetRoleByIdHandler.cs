using Microsoft.EntityFrameworkCore;

using SaaS.Api.Features.Roles.CreateRole;
using SaaS.Api.Infrastructure.Database;

namespace SaaS.Api.Features.Roles.GetRoleById;

public static class GetRoleByIdHandler
{
    public static async Task<IResult> HandleAsync(
        Guid id,
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        var role = await db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Role '{id}' was not found.");

        return Results.Ok(CreateRoleHandler.ToResponse(role));
    }
}
