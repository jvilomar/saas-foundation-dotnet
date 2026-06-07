using System.Net.Http.Headers;
using System.Net.Http.Json;

using SaaS.Api.Features.Auth.Login;
using SaaS.Api.Infrastructure.Database;

namespace SaaS.Api.IntegrationTests.Infrastructure;

internal static class AuthTestHelper
{
    internal static async Task AuthenticateAsSuperAdminAsync(HttpClient client)
    {
        LoginResponse login = await LoginAsync(
            client,
            DatabaseSeeder.DefaultWorkspaceSlug,
            "admin@system.com",
            "Admin123!");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);
    }

    private static async Task<LoginResponse> LoginAsync(
        HttpClient client,
        string workspaceSlug,
        string email,
        string password)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(workspaceSlug, email, password));

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<LoginResponse>())!;
    }
}
