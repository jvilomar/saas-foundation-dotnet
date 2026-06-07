using Microsoft.EntityFrameworkCore;

using SaaS.Api.Features.Users.CreateUser;
using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Database.Entities;

namespace SaaS.Api.Features.Users.UpdateUser;

public static class UpdateUserHandler
{
    public static async Task<IResult> HandleAsync(
        string displayId,
        UpdateUserRequest request,
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        string email = request.Email.Trim().ToLowerInvariant();

        var user = await db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.DisplayId == displayId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{displayId}' was not found.");

        AppRole? role = await db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken)
            ?? throw new KeyNotFoundException($"Role '{request.RoleId}' was not found.");

        if (await db.Users.AnyAsync(
                u => u.Id != user.Id && u.TenantId == user.TenantId && u.Email == email,
                cancellationToken))
        {
            throw new InvalidOperationException("A user with that email already exists in this tenant.");
        }

        user.Email = email;
        user.RoleId = role.Id;

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(CreateUserHandler.ToResponse(user, role.Name));
    }
}
