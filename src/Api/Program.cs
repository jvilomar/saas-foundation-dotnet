using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using SaaS.Api.Features.Auth.Login;
using SaaS.Api.Features.Roles;
using SaaS.Api.Features.Tenants.Create;
using SaaS.Api.Features.Users;
using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Documents;
using SaaS.Api.Infrastructure.Middlewares;
using SaaS.Api.Infrastructure.Security;

using Scalar.AspNetCore;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

bool useSerilog = !builder.Environment.IsEnvironment("Testing");

if (useSerilog)
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();
}

try
{
    if (useSerilog)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
    }

    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

    IConfigurationSection jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
    string jwtSecret = jwtSection["Secret"]
        ?? throw new InvalidOperationException("Jwt:Secret is not configured.");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"] ?? "SaaS.Api",
                ValidAudience = jwtSection["Audience"] ?? "SaaS.Api",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            };
        });

    builder.Services.AddAuthorization();
    builder.Services.AddOpenApi();

    builder.Services.AddScoped<ITenantContext, TenantContext>();
    builder.Services.AddSingleton<JwtTokenService>();
    builder.Services.AddPdfInfrastructure();

    builder.Services.AddScoped<RlsConnectionInterceptor>();
    builder.Services.AddScoped<AuditableEntityInterceptor>();
    builder.Services.AddDbContext<AppDbContext>((sp, options) =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
        options.AddInterceptors(
            sp.GetRequiredService<RlsConnectionInterceptor>(),
            sp.GetRequiredService<AuditableEntityInterceptor>());
    });

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>();

    WebApplication app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseAuthentication();
    app.UseMiddleware<TenantMiddleware>();
    app.UseAuthorization();

    app.MapHealthChecks("/health");
    app.MapLoginEndpoint();
    app.MapCreateTenantEndpoint();
    app.MapRoleEndpoints();
    app.MapUserEndpoints();

    await app.SeedDatabaseAsync();

    app.Run();
}
catch (Exception ex)
{
    if (useSerilog)
    {
        Log.Fatal(ex, "Application terminated unexpectedly");
    }
    else
    {
        Console.Error.WriteLine(ex);
    }
}
finally
{
    if (useSerilog)
    {
        Log.CloseAndFlush();
    }
}

public partial class Program;
