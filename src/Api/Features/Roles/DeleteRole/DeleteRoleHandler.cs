using Microsoft.EntityFrameworkCore;

using SaaS.Api.Infrastructure.Database;

namespace SaaS.Api.Features.Roles.DeleteRole;

public static class DeleteRoleHandler
{
    public static async Task<IResult> HandleAsync(
        Guid id,
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Role '{id}' was not found.");

        if (role.IsSystem)
        {
            throw new InvalidOperationException("System roles cannot be deleted.");
        }

        bool isAssigned = await db.Users.AnyAsync(u => u.RoleId == id, cancellationToken);
        if (isAssigned)
        {
            throw new InvalidOperationException("The role is assigned to one or more users and cannot be deleted.");
        }

        db.Roles.Remove(role);
        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
