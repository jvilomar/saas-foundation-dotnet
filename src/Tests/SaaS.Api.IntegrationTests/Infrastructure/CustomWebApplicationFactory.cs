using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Security;

using Testcontainers.PostgreSql;

namespace SaaS.Api.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16")
        .WithDatabase("saas_integration_test")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await ApplyMigrationsAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = _dbContainer.GetConnectionString(),
            });
        });

        builder.ConfigureTestServices(services =>
        {
            RemoveDbContextRegistrations(services);

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
                options.AddInterceptors(
                    sp.GetRequiredService<RlsConnectionInterceptor>(),
                    sp.GetRequiredService<AuditableEntityInterceptor>());
            });
        });
    }

    private async Task ApplyMigrationsAsync()
    {
        ServiceCollection services = new();
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<RlsConnectionInterceptor>();
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(_dbContainer.GetConnectionString());
            options.AddInterceptors(
                sp.GetRequiredService<RlsConnectionInterceptor>(),
                sp.GetRequiredService<AuditableEntityInterceptor>());
        });

        await using ServiceProvider provider = services.BuildServiceProvider();
        await using AsyncServiceScope scope = provider.CreateAsyncScope();

        ITenantContext tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.EnableBypass();

        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    private static void RemoveDbContextRegistrations(IServiceCollection services)
    {
        ServiceDescriptor[] descriptors = services
            .Where(d =>
                d.ServiceType == typeof(AppDbContext)
                || d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                || d.ServiceType == typeof(DbContextOptions))
            .ToArray();

        foreach (ServiceDescriptor descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }
}
