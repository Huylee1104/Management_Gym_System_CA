using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_Gym_System.Migrations
{
    /// <inheritdoc />
    public partial class EditGymMembershipCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "GymMembershipCards");

            migrationBuilder.AddColumn<int>(
                name: "ThoiHan",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RFID_UID",
                table: "GymMembershipCards",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThoiHan",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RFID_UID",
                table: "GymMembershipCards");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "GymMembershipCards",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
