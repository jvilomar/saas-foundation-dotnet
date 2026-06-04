using Microsoft.EntityFrameworkCore;

using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Database.Entities;
using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Features.Auth.Login;

public static class LoginHandler
{
    public static async Task<IResult> HandleAsync(
        LoginRequest request,
        AppDbContext db,
        JwtTokenService jwtTokenService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.WorkspaceSlug)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Workspace, email, and password are required.");
        }

        string slug = request.WorkspaceSlug.Trim().ToLowerInvariant();
        string email = request.Email.Trim().ToLowerInvariant();

        Tenant? tenant = await db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);

        if (tenant is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        // Login is anonymous: ITenantContext has no tenant yet. Scope by workspace slug instead of the global filter.
#pragma warning disable CA1304, CA1311, CA1862
        User? user = await db.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(
                u => u.TenantId == tenant.Id && u.Email.ToLower() == email,
                cancellationToken);
#pragma warning restore CA1304, CA1311, CA1862

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        string token = jwtTokenService.CreateToken(user);

        return Results.Ok(new LoginResponse(
            Token: token,
            Email: user.Email,
            Role: user.Role.Name,
            UserDisplayId: user.DisplayId,
            TenantDisplayId: tenant.DisplayId,
            WorkspaceSlug: tenant.Slug));
    }
}
