using Microsoft.EntityFrameworkCore;

using SaaS.Api.Infrastructure.Database.Entities;
using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Infrastructure.Database;

public static class DatabaseSeeder
{
    public const string DefaultWorkspaceSlug = "system";

    private const string DefaultTenantName = "System";
    private const string DefaultAdminEmail = "admin@system.com";
    private const string DefaultAdminPassword = "Admin123!";

    private static readonly (Guid Id, string Name, string Description)[] DefaultRoles =
    [
        (new Guid("11111111-1111-4111-8111-111111111101"), RoleNames.SuperAdmin, "Platform-wide administrator"),
        (new Guid("11111111-1111-4111-8111-111111111102"), RoleNames.TenantAdmin, "Administrator within a tenant"),
        (new Guid("11111111-1111-4111-8111-111111111103"), RoleNames.User, "Standard tenant user"),
    ];

    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        ITenantContext tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();

        tenantContext.EnableBypass();

        await SeedRolesAsync(db, app.Logger);

        bool hasTenants = await db.Tenants.IgnoreQueryFilters().AnyAsync();
        if (hasTenants)
        {
            return;
        }

        AppRole superAdminRole = await db.Roles
            .SingleAsync(r => r.Name == RoleNames.SuperAdmin);

        Tenant tenant = new()
        {
            Id = Guid.NewGuid(),
            Slug = DefaultWorkspaceSlug,
            Name = DefaultTenantName,
            CreatedAt = DateTime.UtcNow,
        };

        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        User admin = new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Email = DefaultAdminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DefaultAdminPassword),
            RoleId = superAdminRole.Id,
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync();

        app.Logger.LogInformation(
            "Seeded default workspace {WorkspaceSlug} ({TenantDisplayId}) and user {Email} ({UserDisplayId})",
            tenant.Slug,
            tenant.DisplayId,
            admin.Email,
            admin.DisplayId);
    }

    private static async Task SeedRolesAsync(AppDbContext db, ILogger logger)
    {
        if (await db.Roles.AnyAsync())
        {
            return;
        }

        foreach ((Guid id, string name, string description) in DefaultRoles)
        {
            db.Roles.Add(new AppRole
            {
                Id = id,
                Name = name,
                Description = description,
                IsSystem = true,
            });
        }

        await db.SaveChangesAsync();
        logger.LogInformation("Seeded default roles: {Roles}", string.Join(", ", DefaultRoles.Select(r => r.Name)));
    }
}
