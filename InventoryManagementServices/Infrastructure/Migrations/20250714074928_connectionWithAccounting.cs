using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class connectionWithAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "reason",
                table: "SalesReturnItems",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "voucherNumber",
                table: "SalesReturn",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "voucherNumber",
                table: "Sales",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "voucherNumber",
                table: "Purchases",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "voucherNumber",
                table: "PurchaseReturns",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reason",
                table: "SalesReturnItems");

            migrationBuilder.DropColumn(
                name: "voucherNumber",
                table: "SalesReturn");

            migrationBuilder.DropColumn(
                name: "voucherNumber",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "voucherNumber",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "voucherNumber",
                table: "PurchaseReturns");
        }
    }
}
