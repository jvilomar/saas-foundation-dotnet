using Microsoft.EntityFrameworkCore;

using SaaS.Api.Features.Users.CreateUser;
using SaaS.Api.Infrastructure.Database;

namespace SaaS.Api.Features.Users.GetUserById;

public static class GetUserByIdHandler
{
    public static async Task<IResult> HandleAsync(
        string displayId,
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.DisplayId == displayId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{displayId}' was not found.");

        return Results.Ok(CreateUserHandler.ToResponse(user, user.Role.Name));
    }
}
