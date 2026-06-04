using FluentAssertions;

using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Database.ValueGenerators;

namespace SaaS.Api.Tests.Infrastructure.Database;

public sealed class TenantEntityTests
{
    private const int DisplayIdRandomLength = 8;
    private const string TenantDisplayIdPrefix = "ten_";
    private static readonly int ExpectedTenantDisplayIdLength = TenantDisplayIdPrefix.Length + DisplayIdRandomLength;

    [Fact]
    public void NewTenant_DisplayIdAssignedViaGenerator_HasStripePattern()
    {
        var valueGenerator = new TenantDisplayIdValueGenerator();
        string displayId = valueGenerator.Next(null!);

        displayId.Should().StartWith(TenantDisplayIdPrefix);
        displayId.Should().HaveLength(ExpectedTenantDisplayIdLength);
        displayId[TenantDisplayIdPrefix.Length..].Should().MatchRegex("^[A-Za-z0-9_-]{8}$");
    }

    [Fact]
    public void DisplayIdGenerator_ProducesUniqueTenantDisplayIds()
    {
        HashSet<string> ids = Enumerable.Range(0, 50)
            .Select(_ => DisplayIdGenerator.NewTenantDisplayId())
            .ToHashSet(StringComparer.Ordinal);

        ids.Should().HaveCount(50);
        ids.Should().OnlyContain(id => id.StartsWith(TenantDisplayIdPrefix, StringComparison.Ordinal));
    }

    [Fact]
    public void DisplayIdGenerator_UserDisplayId_HasStripePattern()
    {
        const string userPrefix = "usr_";
        string displayId = DisplayIdGenerator.NewUserDisplayId();

        displayId.Should().StartWith(userPrefix);
        displayId.Should().HaveLength(userPrefix.Length + DisplayIdRandomLength);
    }
}
