using Microsoft.EntityFrameworkCore;

using Shouldly;

using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Database.Entities;
using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Tests.Infrastructure.Database;

public sealed class AuditableEntityInterceptorTests
{
    private static readonly Guid ActorUserId = new("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    [Fact]
    public async Task SaveChanges_UsesTenantContextUserId_WhenUserIsAuthenticated()
    {
        StubTenantContext tenantContext = new();
        tenantContext.SetUserId(ActorUserId);
        tenantContext.EnableBypass();

        await using AppDbContext db = CreateContext(tenantContext);

        Tenant tenant = new()
        {
            Id = Guid.NewGuid(),
            Slug = "audit-auth",
            Name = "Audit Auth Tenant",
        };

        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        tenant.CreatedBy.ShouldBe(ActorUserId.ToString("D"));
        tenant.LastModifiedBy.ShouldBe(ActorUserId.ToString("D"));
        tenant.CreatedAt.ShouldBeGreaterThan(DateTimeOffset.MinValue);
        tenant.LastModifiedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task SaveChanges_UsesSystemActor_WhenTenantContextHasNoUser()
    {
        StubTenantContext tenantContext = new();
        tenantContext.EnableBypass();

        await using AppDbContext db = CreateContext(tenantContext);

        Tenant tenant = new()
        {
            Id = Guid.NewGuid(),
            Slug = "audit-system",
            Name = "Audit System Tenant",
        };

        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        tenant.CreatedBy.ShouldBe("system");
        tenant.LastModifiedBy.ShouldBe("system");
    }

    [Fact]
    public async Task SaveChanges_DoesNotOverwriteCreatedFields_WhenEntityIsModified()
    {
        StubTenantContext tenantContext = new();
        tenantContext.SetUserId(ActorUserId);
        tenantContext.EnableBypass();

        await using AppDbContext db = CreateContext(tenantContext);

        Tenant tenant = new()
        {
            Id = Guid.NewGuid(),
            Slug = "audit-update",
            Name = "Original Name",
        };

        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();

        DateTimeOffset originalCreatedAt = tenant.CreatedAt;
        string originalCreatedBy = tenant.CreatedBy!;

        Guid updaterUserId = new("ffffffff-1111-2222-3333-444444444444");
        tenantContext.SetUserId(updaterUserId);
        tenant.Name = "Updated Name";
        await db.SaveChangesAsync();

        tenant.CreatedAt.ShouldBe(originalCreatedAt);
        tenant.CreatedBy.ShouldBe(originalCreatedBy);
        tenant.LastModifiedBy.ShouldBe(updaterUserId.ToString("D"));
        tenant.LastModifiedAt.ShouldNotBeNull();
        tenant.LastModifiedAt!.Value.ShouldBeGreaterThanOrEqualTo(originalCreatedAt);
    }

    private static AppDbContext CreateContext(StubTenantContext tenantContext)
    {
        AuditableEntityInterceptor interceptor = new(tenantContext);

        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;

        return new AppDbContext(options, tenantContext);
    }

    private sealed class StubTenantContext : ITenantContext
    {
        public Guid? TenantId { get; private set; }

        public Guid? UserId { get; private set; }

        public bool BypassTenantIsolation { get; private set; }

        public void SetTenantId(Guid tenantId) => TenantId = tenantId;

        public void SetUserId(Guid userId) => UserId = userId;

        public void EnableBypass() => BypassTenantIsolation = true;
    }
}
