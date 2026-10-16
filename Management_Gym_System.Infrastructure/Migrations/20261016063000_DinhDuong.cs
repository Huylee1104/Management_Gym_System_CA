using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Management_Gym_System.Migrations
{
    /// <inheritdoc />
    public partial class DinhDuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NutritionAnalyzeHistorys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemberId = table.Column<long>(type: "bigint", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    FoodName = table.Column<string>(type: "text", nullable: true),
                    EstimatedWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    WeightUnit = table.Column<string>(type: "text", nullable: true),
                    EstimatedCalories = table.Column<decimal>(type: "numeric", nullable: true),
                    EstimatedProtein = table.Column<decimal>(type: "numeric", nullable: true),
                    EstimatedCarb = table.Column<decimal>(type: "numeric", nullable: true),
                    EstimatedFat = table.Column<decimal>(type: "numeric", nullable: true),
                    AnalyzeResult = table.Column<string>(type: "text", nullable: true),
                    Confidence = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionAnalyzeHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionAnalyzeHistorys_Users_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NutritionGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NutritionDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NutritionGroupId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Calories = table.Column<decimal>(type: "numeric", nullable: false),
                    Protein = table.Column<decimal>(type: "numeric", nullable: false),
                    Carb = table.Column<decimal>(type: "numeric", nullable: false),
                    Fat = table.Column<decimal>(type: "numeric", nullable: false),
                    Fiber = table.Column<decimal>(type: "numeric", nullable: false),
                    Sugar = table.Column<decimal>(type: "numeric", nullable: false),
                    Sodium = table.Column<decimal>(type: "numeric", nullable: false),
                    ServingSize = table.Column<decimal>(type: "numeric", nullable: false),
                    ServingUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RecommendedTime = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RecommendedTarget = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Pros = table.Column<string>(type: "text", nullable: true),
                    Cons = table.Column<string>(type: "text", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionDetails_NutritionGroups_NutritionGroupId",
                        column: x => x.NutritionGroupId,
                        principalTable: "NutritionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NutritionAnalyzeHistorys_MemberId",
                table: "NutritionAnalyzeHistorys",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionDetails_NutritionGroupId",
                table: "NutritionDetails",
                column: "NutritionGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NutritionAnalyzeHistorys");

            migrationBuilder.DropTable(
                name: "NutritionDetails");

            migrationBuilder.DropTable(
                name: "NutritionGroups");
        }
    }
}
