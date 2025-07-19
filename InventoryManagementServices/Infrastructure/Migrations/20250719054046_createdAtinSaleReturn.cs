using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createdAtinSaleReturn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "voucherNumber",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "voucherNumber",
                table: "Purchases");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SalesReturn",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "CashCustomers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SalesReturn");

            migrationBuilder.DropColumn(
                name: "Place",
                table: "CashCustomers");

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
        }
    }
}
