namespace SaaS.Api.Infrastructure.Database.Entities;

public sealed class User
{
    public Guid Id { get; set; }

    public string DisplayId { get; set; } = string.Empty;

    public Guid TenantId { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public Guid RoleId { get; set; }

    public AppRole Role { get; set; } = null!;

    public Tenant Tenant { get; set; } = null!;
}
