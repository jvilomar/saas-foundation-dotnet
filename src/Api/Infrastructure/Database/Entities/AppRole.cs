using SaaS.Api.Domain.Abstractions;

namespace SaaS.Api.Infrastructure.Database.Entities;

public sealed class AppRole : IAuditableEntity
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public bool IsSystem { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? LastModifiedAt { get; set; }

    public string? LastModifiedBy { get; set; }
}
