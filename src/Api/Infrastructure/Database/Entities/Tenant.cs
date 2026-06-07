using SaaS.Api.Domain.Abstractions;

namespace SaaS.Api.Infrastructure.Database.Entities;

public sealed class Tenant : IAuditableEntity
{
    public Guid Id { get; set; }

    public string DisplayId { get; set; } = null!;

    public required string Slug { get; set; }

    public required string Name { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? LastModifiedAt { get; set; }

    public string? LastModifiedBy { get; set; }
}
