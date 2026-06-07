using SaaS.Api.Domain.Abstractions;

namespace SaaS.Api.Infrastructure.Database.Entities;

public sealed class User : IAuditableEntity
{
    public Guid Id { get; set; }

    public string DisplayId { get; set; } = null!;

    public Guid TenantId { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public Guid RoleId { get; set; }

    public AppRole Role { get; set; } = null!;

    public Tenant Tenant { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? LastModifiedAt { get; set; }

    public string? LastModifiedBy { get; set; }
}
