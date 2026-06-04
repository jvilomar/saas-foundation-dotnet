namespace SaaS.Api.Infrastructure.Security;

public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }

    public Guid? UserId { get; private set; }

    public bool BypassTenantIsolation { get; private set; }

    public void SetTenantId(Guid tenantId) => TenantId = tenantId;

    public void SetUserId(Guid userId) => UserId = userId;

    public void EnableBypass() => BypassTenantIsolation = true;
}
