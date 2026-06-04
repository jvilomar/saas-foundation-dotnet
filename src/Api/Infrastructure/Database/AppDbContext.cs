using Microsoft.EntityFrameworkCore;

using SaaS.Api.Infrastructure.Database.Entities;
using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Infrastructure.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
    : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<AppRole> Roles => Set<AppRole>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplyTenantQueryFilters(modelBuilder);
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasQueryFilter(u =>
            tenantContext.BypassTenantIsolation
            || (tenantContext.TenantId != null && u.TenantId == tenantContext.TenantId));
    }
}
