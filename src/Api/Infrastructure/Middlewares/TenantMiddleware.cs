using System.Security.Claims;

using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Infrastructure.Middlewares;

public sealed class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        string? userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out Guid userId))
        {
            tenantContext.SetUserId(userId);
        }

        string? tenantClaim = context.User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantClaim)
            && context.Request.Headers.TryGetValue("X-Tenant-ID", out Microsoft.Extensions.Primitives.StringValues headerTenant))
        {
            tenantClaim = headerTenant.ToString();
        }

        if (Guid.TryParse(tenantClaim, out Guid tenantId))
        {
            tenantContext.SetTenantId(tenantId);
        }

        if (context.User.IsInRole(RoleNames.SuperAdmin))
        {
            tenantContext.EnableBypass();
        }

        await next(context);
    }
}
