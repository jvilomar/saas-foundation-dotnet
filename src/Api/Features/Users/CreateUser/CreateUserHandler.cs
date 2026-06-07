using Microsoft.EntityFrameworkCore;

using SaaS.Api.Features.Users;
using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Database.Entities;
using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Users.CreateUser;

public static class CreateUserHandler
{
    public static async Task<IResult> HandleAsync(
        CreateUserRequest request,
        AppDbContext db,
        ITenantContext tenantContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Email and password are required.");
        }

        if (tenantContext.TenantId is not Guid tenantId)
        {
            throw new UnauthorizedAccessException("Tenant context is required to create a user.");
        }

        string email = request.Email.Trim().ToLowerInvariant();

        AppRole? role = await db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken)
            ?? throw new KeyNotFoundException($"Role '{request.RoleId}' was not found.");

        if (await db.Users.AnyAsync(u => u.TenantId == tenantId && u.Email == email, cancellationToken))
        {
            throw new InvalidOperationException("A user with that email already exists in this tenant.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = role.Id,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return Results.Created(
            $"/api/users/{user.DisplayId}",
            ToResponse(user, role.Name));
    }

    internal static UserResponse ToResponse(User user, string roleName) =>
        new(
            user.DisplayId,
            user.Email,
            user.TenantId,
            user.RoleId,
            roleName,
            user.CreatedAt,
            user.LastModifiedAt);
}
