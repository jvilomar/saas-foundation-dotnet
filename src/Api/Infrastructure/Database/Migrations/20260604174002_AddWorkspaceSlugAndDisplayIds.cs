using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaaS.Api.Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddWorkspaceSlugAndDisplayIds : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "DisplayId",
            table: "users",
            type: "character varying(16)",
            maxLength: 16,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DisplayId",
            table: "tenants",
            type: "character varying(16)",
            maxLength: 16,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Slug",
            table: "tenants",
            type: "character varying(64)",
            maxLength: 64,
            nullable: true);

        migrationBuilder.Sql(
            """
            UPDATE tenants
            SET "Slug" = CASE
                WHEN lower(trim("Name")) = 'system' THEN 'system'
                ELSE trim(both '-' from lower(regexp_replace(trim("Name"), '[^a-zA-Z0-9]+', '-', 'g')))
            END,
            "DisplayId" = 'ten_' || substr(md5("Id"::text), 1, 8)
            WHERE "Slug" IS NULL OR "DisplayId" IS NULL;
            """);

        migrationBuilder.Sql(
            """
            UPDATE users
            SET "DisplayId" = 'usr_' || substr(md5("Id"::text), 1, 8)
            WHERE "DisplayId" IS NULL;
            """);

        migrationBuilder.AlterColumn<string>(
            name: "DisplayId",
            table: "users",
            type: "character varying(16)",
            maxLength: 16,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(16)",
            oldMaxLength: 16,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DisplayId",
            table: "tenants",
            type: "character varying(16)",
            maxLength: 16,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(16)",
            oldMaxLength: 16,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Slug",
            table: "tenants",
            type: "character varying(64)",
            maxLength: 64,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(64)",
            oldMaxLength: 64,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_users_DisplayId",
            table: "users",
            column: "DisplayId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_tenants_DisplayId",
            table: "tenants",
            column: "DisplayId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_tenants_Slug",
            table: "tenants",
            column: "Slug",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_users_DisplayId",
            table: "users");

        migrationBuilder.DropIndex(
            name: "IX_tenants_DisplayId",
            table: "tenants");

        migrationBuilder.DropIndex(
            name: "IX_tenants_Slug",
            table: "tenants");

        migrationBuilder.DropColumn(
            name: "DisplayId",
            table: "users");

        migrationBuilder.DropColumn(
            name: "DisplayId",
            table: "tenants");

        migrationBuilder.DropColumn(
            name: "Slug",
            table: "tenants");
    }
}
