using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Management_Gym_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkoutSchedules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CardId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PauseDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ResumeDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CheckinTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSchedules_GymMembershipCards_CardId",
                        column: x => x.CardId,
                        principalTable: "GymMembershipCards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkoutSchedules_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutNoteDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    MemberNote = table.Column<string>(type: "text", nullable: true),
                    MemberNoteDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TrainerNote = table.Column<string>(type: "text", nullable: true),
                    TrainerNoteDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UserID = table.Column<long>(type: "bigint", nullable: true),
                    WorkoutScheduleId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutNoteDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutNoteDetails_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_WorkoutNoteDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkoutNoteDetails_WorkoutSchedules_WorkoutScheduleId",
                        column: x => x.WorkoutScheduleId,
                        principalTable: "WorkoutSchedules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutNoteDetails_UserId",
                table: "WorkoutNoteDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutNoteDetails_UserID",
                table: "WorkoutNoteDetails",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutNoteDetails_WorkoutScheduleId",
                table: "WorkoutNoteDetails",
                column: "WorkoutScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSchedules_CardId",
                table: "WorkoutSchedules",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSchedules_UserId",
                table: "WorkoutSchedules",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutNoteDetails");

            migrationBuilder.DropTable(
                name: "WorkoutSchedules");
        }
    }
}
