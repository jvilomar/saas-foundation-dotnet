using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.Extensions.Options;

using Shouldly;

using SaaS.Api.Infrastructure.Database.Entities;
using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Tests.Infrastructure.Security;

public sealed class JwtTokenServiceTests
{
    private static readonly Guid TenantId = new("11111111-2222-3333-4444-555555555555");
    private static readonly Guid UserId = new("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    [Fact]
    public void CreateToken_IncludesTenantIdAndRoleClaims()
    {
        JwtTokenService service = new(Options.Create(CreateJwtOptions()));
        User user = new()
        {
            Id = UserId,
            TenantId = TenantId,
            Email = "admin@system.com",
            PasswordHash = "hash",
            Role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = RoleNames.SuperAdmin,
            },
        };

        string token = service.CreateToken(user);
        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Claims.First(c => c.Type == "tenant_id").Value.ShouldBe(TenantId.ToString());
        jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value.ShouldBe(RoleNames.SuperAdmin);
        jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value.ShouldBe(UserId.ToString());
    }

    private static JwtOptions CreateJwtOptions() => new()
    {
        Secret = "integration-test-secret-at-least-32-characters-long",
        Issuer = "SaaS.Api.Tests",
        Audience = "SaaS.Api.Tests",
        ExpirationHours = 1,
    };
}
