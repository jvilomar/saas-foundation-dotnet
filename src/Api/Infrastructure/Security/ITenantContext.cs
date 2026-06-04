namespace SaaS.Api.Infrastructure.Security;

public interface ITenantContext
{
    Guid? TenantId { get; }

    Guid? UserId { get; }

    bool BypassTenantIsolation { get; }

    void SetTenantId(Guid tenantId);

    void SetUserId(Guid userId);

    void EnableBypass();
}
