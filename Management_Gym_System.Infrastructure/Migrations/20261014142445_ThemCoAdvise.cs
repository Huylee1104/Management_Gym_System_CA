using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_Gym_System.Migrations
{
    /// <inheritdoc />
    public partial class ThemCoAdvise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StaffId",
                table: "MemberAdvises",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberAdvises_ProductId",
                table: "MemberAdvises",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAdvises_StaffId",
                table: "MemberAdvises",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberAdvises_Products_ProductId",
                table: "MemberAdvises",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberAdvises_Users_StaffId",
                table: "MemberAdvises",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberAdvises_Products_ProductId",
                table: "MemberAdvises");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberAdvises_Users_StaffId",
                table: "MemberAdvises");

            migrationBuilder.DropIndex(
                name: "IX_MemberAdvises_ProductId",
                table: "MemberAdvises");

            migrationBuilder.DropIndex(
                name: "IX_MemberAdvises_StaffId",
                table: "MemberAdvises");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "MemberAdvises");
        }
    }
}
