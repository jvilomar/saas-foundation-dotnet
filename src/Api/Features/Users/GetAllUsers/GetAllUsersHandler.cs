using Microsoft.EntityFrameworkCore;

using SaaS.Api.Features.Users.CreateUser;
using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Database.Entities;

namespace SaaS.Api.Features.Users.GetAllUsers;

public static class GetAllUsersHandler
{
    public static async Task<IResult> HandleAsync(
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        List<User> users = await db.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return Results.Ok(users.Select(u => CreateUserHandler.ToResponse(u, u.Role.Name)));
    }
}
