using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_Gym_System.Migrations
{
    /// <inheritdoc />
    public partial class DoitenThuocTinh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMember",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "Users",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsMember",
                table: "Users",
                type: "boolean",
                nullable: true);
        }
    }
}
