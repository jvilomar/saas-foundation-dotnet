using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaaS.Api.Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddRolesTable : Migration
{
    private static readonly Guid SuperAdminRoleId = new("11111111-1111-4111-8111-111111111101");
    private static readonly Guid TenantAdminRoleId = new("11111111-1111-4111-8111-111111111102");
    private static readonly Guid UserRoleId = new("11111111-1111-4111-8111-111111111103");

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "roles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                IsSystem = table.Column<bool>(type: "boolean", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_roles", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_roles_Name",
            table: "roles",
            column: "Name",
            unique: true);

        migrationBuilder.InsertData(
            table: "roles",
            columns: new[] { "Id", "Name", "Description", "IsSystem" },
            values: new object[,]
            {
                { SuperAdminRoleId, "SuperAdmin", "Platform-wide administrator", true },
                { TenantAdminRoleId, "TenantAdmin", "Administrator within a tenant", true },
                { UserRoleId, "User", "Standard tenant user", true },
            });

        migrationBuilder.AddColumn<Guid>(
            name: "RoleId",
            table: "users",
            type: "uuid",
            nullable: true);

        migrationBuilder.Sql(
            """
            UPDATE users u
            SET "RoleId" = r."Id"
            FROM roles r
            WHERE r."Name" = u."Role";
            """);

        migrationBuilder.Sql(
            $"""
            UPDATE users
            SET "RoleId" = '{SuperAdminRoleId:D}'
            WHERE "RoleId" IS NULL;
            """);

        migrationBuilder.AlterColumn<Guid>(
            name: "RoleId",
            table: "users",
            type: "uuid",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);

        migrationBuilder.DropColumn(
            name: "Role",
            table: "users");

        migrationBuilder.CreateIndex(
            name: "IX_users_RoleId",
            table: "users",
            column: "RoleId");

        migrationBuilder.AddForeignKey(
            name: "FK_users_roles_RoleId",
            table: "users",
            column: "RoleId",
            principalTable: "roles",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_users_roles_RoleId",
            table: "users");

        migrationBuilder.DropIndex(
            name: "IX_users_RoleId",
            table: "users");

        migrationBuilder.AddColumn<string>(
            name: "Role",
            table: "users",
            type: "character varying(32)",
            maxLength: 32,
            nullable: false,
            defaultValue: "");

        migrationBuilder.Sql(
            """
            UPDATE users u
            SET "Role" = r."Name"
            FROM roles r
            WHERE r."Id" = u."RoleId";
            """);

        migrationBuilder.DropColumn(
            name: "RoleId",
            table: "users");

        migrationBuilder.DropTable(
            name: "roles");
    }
}
