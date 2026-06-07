using Microsoft.Extensions.DependencyInjection;

using SaaS.Api.Infrastructure.Database;

namespace SaaS.Api.IntegrationTests.Infrastructure;

[Collection(IntegrationTestCollection.Name)]
public abstract class BaseIntegrationTest
{
    protected HttpClient Client { get; }

    protected CustomWebApplicationFactory Factory { get; }

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected AsyncServiceScope CreateDbContextScope()
        => Factory.Services.CreateAsyncScope();

    protected static AppDbContext GetDbContext(AsyncServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<AppDbContext>();
}
