using Microsoft.EntityFrameworkCore;

using SaaS.Api.Infrastructure.Database;

namespace SaaS.Api.Features.Users.DeleteUser;

public static class DeleteUserHandler
{
    public static async Task<IResult> HandleAsync(
        string displayId,
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.DisplayId == displayId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{displayId}' was not found.");

        db.Users.Remove(user);
        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
