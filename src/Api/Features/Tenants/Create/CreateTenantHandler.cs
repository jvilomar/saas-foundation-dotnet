using Microsoft.EntityFrameworkCore;

using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Database.Entities;

namespace SaaS.Api.Features.Tenants.Create;

public static class CreateTenantHandler
{
    public static async Task<IResult> HandleAsync(
        CreateTenantRequest request,
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Tenant name is required.");
        }

        string name = request.Name.Trim();
        string slug = SlugGenerator.FromName(name);

        if (await db.Tenants.AnyAsync(t => t.Name == name, cancellationToken))
        {
            throw new InvalidOperationException("A tenant with that name already exists.");
        }

        if (await db.Tenants.AnyAsync(t => t.Slug == slug, cancellationToken))
        {
            slug = $"{slug}-{Guid.NewGuid().ToString("N")[..6]}";
        }

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Slug = slug,
            Name = name,
            CreatedAt = DateTime.UtcNow,
        };

        db.Tenants.Add(tenant);
        await db.SaveChangesAsync(cancellationToken);

        return Results.Created(
            $"/api/tenants/{tenant.DisplayId}",
            new CreateTenantResponse(
                tenant.DisplayId,
                tenant.Slug,
                tenant.Name,
                tenant.CreatedAt));
    }
}
