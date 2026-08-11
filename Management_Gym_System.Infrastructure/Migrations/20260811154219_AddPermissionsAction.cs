using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Management_Gym_System.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_SystemFunctions_FunctionId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_UserRoles_RoleId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_FunctionId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId_FunctionId",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CanCreate",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CanDelete",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CanEdit",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CanExport",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "FunctionId",
                table: "RolePermissions");

            migrationBuilder.RenameColumn(
                name: "CanView",
                table: "RolePermissions",
                newName: "IsAllowed");

            migrationBuilder.AddColumn<long>(
                name: "ActionId",
                table: "RolePermissions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "SystemFunctionActions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FunctionId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ActionName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemFunctionActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemFunctionActions_SystemFunctions_FunctionId",
                        column: x => x.FunctionId,
                        principalTable: "SystemFunctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_ActionId",
                table: "RolePermissions",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_ActionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "ActionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemFunctionActions_Code",
                table: "SystemFunctionActions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemFunctionActions_FunctionId",
                table: "SystemFunctionActions",
                column: "FunctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_SystemFunctionActions_ActionId",
                table: "RolePermissions",
                column: "ActionId",
                principalTable: "SystemFunctionActions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_UserRoles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "UserRoles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_SystemFunctionActions_ActionId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_UserRoles_RoleId",
                table: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SystemFunctionActions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_ActionId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId_ActionId",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ActionId",
                table: "RolePermissions");

            migrationBuilder.RenameColumn(
                name: "IsAllowed",
                table: "RolePermissions",
                newName: "CanView");

            migrationBuilder.AddColumn<bool>(
                name: "CanCreate",
                table: "RolePermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanDelete",
                table: "RolePermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanEdit",
                table: "RolePermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanExport",
                table: "RolePermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "FunctionId",
                table: "RolePermissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_FunctionId",
                table: "RolePermissions",
                column: "FunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_FunctionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "FunctionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_SystemFunctions_FunctionId",
                table: "RolePermissions",
                column: "FunctionId",
                principalTable: "SystemFunctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_UserRoles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "UserRoles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
