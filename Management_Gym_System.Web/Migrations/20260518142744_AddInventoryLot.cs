using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Management_Gym_System.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryLot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchCode",
                table: "ImportReceiptDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "ImportReceiptDetails",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDiscount",
                table: "ImportReceiptDetails",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalTax",
                table: "ImportReceiptDetails",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExportPrice",
                table: "ExportReceiptDetails",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "ExportReceiptDetails",
                type: "numeric(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "ExportReceiptDetails",
                type: "numeric(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDiscount",
                table: "ExportReceiptDetails",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalTax",
                table: "ExportReceiptDetails",
                type: "numeric(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchCode",
                table: "ImportReceiptDetails");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "ImportReceiptDetails");

            migrationBuilder.DropColumn(
                name: "TotalDiscount",
                table: "ImportReceiptDetails");

            migrationBuilder.DropColumn(
                name: "TotalTax",
                table: "ImportReceiptDetails");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "ExportReceiptDetails");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "ExportReceiptDetails");

            migrationBuilder.DropColumn(
                name: "TotalDiscount",
                table: "ExportReceiptDetails");

            migrationBuilder.DropColumn(
                name: "TotalTax",
                table: "ExportReceiptDetails");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExportPrice",
                table: "ExportReceiptDetails",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);
        }
    }
}
