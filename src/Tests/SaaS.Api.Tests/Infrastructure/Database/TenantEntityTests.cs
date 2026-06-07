using Shouldly;

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

        displayId.ShouldStartWith(TenantDisplayIdPrefix);
        displayId.Length.ShouldBe(ExpectedTenantDisplayIdLength);
        displayId[TenantDisplayIdPrefix.Length..].ShouldMatch("^[A-Za-z0-9_-]{8}$");
    }

    [Fact]
    public void DisplayIdGenerator_ProducesUniqueTenantDisplayIds()
    {
        HashSet<string> ids = Enumerable.Range(0, 50)
            .Select(_ => DisplayIdGenerator.NewTenantDisplayId())
            .ToHashSet(StringComparer.Ordinal);

        ids.Count.ShouldBe(50);
        ids.ShouldAllBe(id => id.StartsWith(TenantDisplayIdPrefix, StringComparison.Ordinal));
    }

    [Fact]
    public void DisplayIdGenerator_UserDisplayId_HasStripePattern()
    {
        const string userPrefix = "usr_";
        string displayId = DisplayIdGenerator.NewUserDisplayId();

        displayId.ShouldStartWith(userPrefix);
        displayId.Length.ShouldBe(userPrefix.Length + DisplayIdRandomLength);
    }
}
