using Microsoft.EntityFrameworkCore;

using SaaS.Api.Features.Roles;
using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Database.Entities;

namespace SaaS.Api.Features.Roles.CreateRole;

public static class CreateRoleHandler
{
    public static async Task<IResult> HandleAsync(
        CreateRoleRequest request,
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Role name is required.");
        }

        string name = request.Name.Trim();

        if (await db.Roles.AnyAsync(r => r.Name == name, cancellationToken))
        {
            throw new InvalidOperationException("A role with that name already exists.");
        }

        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = request.Description?.Trim(),
            IsSystem = false,
        };

        db.Roles.Add(role);
        await db.SaveChangesAsync(cancellationToken);

        return Results.Created(
            $"/api/roles/{role.Id}",
            ToResponse(role));
    }

    internal static RoleResponse ToResponse(AppRole role) =>
        new(
            role.Id,
            role.Name,
            role.Description,
            role.IsSystem,
            role.CreatedAt,
            role.LastModifiedAt);
}
