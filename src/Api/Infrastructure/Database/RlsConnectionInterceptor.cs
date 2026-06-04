using System.Data.Common;

using Microsoft.EntityFrameworkCore.Diagnostics;

using SaaS.Api.Infrastructure.Security;

namespace SaaS.Api.Infrastructure.Database;

/// <summary>
/// Sets PostgreSQL session variables consumed by row-level security policies.
/// </summary>
public sealed class RlsConnectionInterceptor(ITenantContext tenantContext) : DbConnectionInterceptor
{
    private const string TenantSetting = "app.current_tenant_id";
    private const string BypassSetting = "app.bypass_rls";

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        await ApplySessionSettingsAsync(connection, cancellationToken);
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        ApplySessionSettings(connection);
        base.ConnectionOpened(connection, eventData);
    }

    private void ApplySessionSettings(DbConnection connection)
    {
        using DbCommand command = connection.CreateCommand();

        if (tenantContext.BypassTenantIsolation)
        {
            command.CommandText = $"SET {BypassSetting} = 'true'; RESET {TenantSetting};";
        }
        else if (tenantContext.TenantId is Guid tenantId)
        {
            command.CommandText =
                $"RESET {BypassSetting}; SET {TenantSetting} = '{tenantId:D}';";
        }
        else
        {
            command.CommandText = $"RESET {BypassSetting}; RESET {TenantSetting};";
        }

        command.ExecuteNonQuery();
    }

    private async Task ApplySessionSettingsAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        await using DbCommand command = connection.CreateCommand();

        if (tenantContext.BypassTenantIsolation)
        {
            command.CommandText = $"SET {BypassSetting} = 'true'; RESET {TenantSetting};";
        }
        else if (tenantContext.TenantId is Guid tenantId)
        {
            command.CommandText =
                $"RESET {BypassSetting}; SET {TenantSetting} = '{tenantId:D}';";
        }
        else
        {
            command.CommandText = $"RESET {BypassSetting}; RESET {TenantSetting};";
        }

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
