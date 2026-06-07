using System.Net;

using Shouldly;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SaaS.Api.Infrastructure.Database;
using SaaS.Api.IntegrationTests.Infrastructure;

namespace SaaS.Api.IntegrationTests;

public sealed class DatabaseConnectionTests(CustomWebApplicationFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Health_ReturnsOk_WhenDatabaseContainerIsRunning()
    {
        HttpResponseMessage response = await Client.GetAsync("/health");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using AsyncServiceScope scope = CreateDbContextScope();
        AppDbContext db = GetDbContext(scope);

        (await db.Database.CanConnectAsync()).ShouldBeTrue();
    }
}
