using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_Gym_System.Migrations
{
    /// <inheritdoc />
    public partial class ThemThuocTinhUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GioiTinh",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMember",
                table: "Users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySinh",
                table: "Users",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GioiTinh",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsMember",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NgaySinh",
                table: "Users");
        }
    }
}
