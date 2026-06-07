using System.Net;
using System.Net.Http.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using SaaS.Api.Features.Roles;
using SaaS.Api.Features.Roles.CreateRole;
using SaaS.Api.Features.Users;
using SaaS.Api.Features.Users.CreateUser;
using SaaS.Api.Infrastructure.Database;
using SaaS.Api.Infrastructure.Security;
using SaaS.Api.IntegrationTests.Infrastructure;

namespace SaaS.Api.IntegrationTests;

public sealed class UserAndRoleEndpointsTests(CustomWebApplicationFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllRoles_ReturnsSeededRoles_WhenAuthenticatedAsSuperAdmin()
    {
        await AuthTestHelper.AuthenticateAsSuperAdminAsync(Client);

        HttpResponseMessage response = await Client.GetAsync("/api/roles");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        List<RoleResponse> roles = (await response.Content.ReadFromJsonAsync<List<RoleResponse>>())!;
        roles.ShouldNotBeEmpty();
        roles.ShouldContain(r => r.Name == RoleNames.SuperAdmin);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreated_AndPersistsUserInCurrentTenant()
    {
        await AuthTestHelper.AuthenticateAsSuperAdminAsync(Client);

        HttpResponseMessage rolesResponse = await Client.GetAsync("/api/roles");
        rolesResponse.EnsureSuccessStatusCode();
        List<RoleResponse> roles = (await rolesResponse.Content.ReadFromJsonAsync<List<RoleResponse>>())!;
        Guid userRoleId = roles.Single(r => r.Name == RoleNames.User).Id;

        string email = $"user-{Guid.NewGuid():N}@system.com";
        HttpResponseMessage createResponse = await Client.PostAsJsonAsync(
            "/api/users",
            new CreateUserRequest(email, "Password123!", userRoleId));

        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        UserResponse created = (await createResponse.Content.ReadFromJsonAsync<UserResponse>())!;
        created.Email.ShouldBe(email);
        created.RoleName.ShouldBe(RoleNames.User);
        created.CreatedAt.ShouldBeGreaterThan(DateTimeOffset.MinValue);

        await using AsyncServiceScope scope = CreateDbContextScope();
        AppDbContext db = GetDbContext(scope);
        ITenantContext tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
        tenantContext.SetTenantId(created.TenantId);

        bool exists = await db.Users.AnyAsync(u => u.DisplayId == created.DisplayId && u.Email == email);
        exists.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateRole_ReturnsCreated_AndPersistsRole()
    {
        await AuthTestHelper.AuthenticateAsSuperAdminAsync(Client);

        string roleName = $"CustomRole-{Guid.NewGuid():N}"[..24];
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            "/api/roles",
            new CreateRoleRequest(roleName, "Integration test role"));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        RoleResponse created = (await response.Content.ReadFromJsonAsync<RoleResponse>())!;
        created.Name.ShouldBe(roleName);
        created.IsSystem.ShouldBeFalse();

        await using AsyncServiceScope scope = CreateDbContextScope();
        AppDbContext db = GetDbContext(scope);

        bool exists = await db.Roles.AnyAsync(r => r.Id == created.Id && r.Name == roleName);
        exists.ShouldBeTrue();
    }
}
