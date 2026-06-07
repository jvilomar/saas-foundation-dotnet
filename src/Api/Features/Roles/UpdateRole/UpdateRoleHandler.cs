using Microsoft.EntityFrameworkCore;

using SaaS.Api.Features.Roles.CreateRole;
using SaaS.Api.Infrastructure.Database;

namespace SaaS.Api.Features.Roles.UpdateRole;

public static class UpdateRoleHandler
{
    public static async Task<IResult> HandleAsync(
        Guid id,
        UpdateRoleRequest request,
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Role name is required.");
        }

        string name = request.Name.Trim();

        var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Role '{id}' was not found.");

        if (role.IsSystem)
        {
            throw new InvalidOperationException("System roles cannot be modified.");
        }

        if (await db.Roles.AnyAsync(r => r.Id != id && r.Name == name, cancellationToken))
        {
            throw new InvalidOperationException("A role with that name already exists.");
        }

        role.Name = name;
        role.Description = request.Description?.Trim();

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(CreateRoleHandler.ToResponse(role));
    }
}
