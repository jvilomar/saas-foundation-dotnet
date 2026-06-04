namespace SaaS.Api.Infrastructure.Database.Entities;

public sealed class Tenant
{
    public Guid Id { get; set; }

    public string DisplayId { get; set; } = string.Empty;

    public required string Slug { get; set; }

    public required string Name { get; set; }

    public DateTime CreatedAt { get; set; }
}
