namespace SaaS.Api.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Secret { get; init; }

    public string Issuer { get; init; } = "SaaS.Api";

    public string Audience { get; init; } = "SaaS.Api";

    public int ExpirationHours { get; init; } = 8;
}
