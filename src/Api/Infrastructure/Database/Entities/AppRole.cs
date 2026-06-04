namespace SaaS.Api.Infrastructure.Database.Entities;

public sealed class AppRole
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public bool IsSystem { get; set; }
}
