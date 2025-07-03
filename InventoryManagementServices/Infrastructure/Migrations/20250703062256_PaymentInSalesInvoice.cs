using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentInSalesInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Sales");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "SalesInvoices",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RecievedAmount",
                table: "SalesInvoices",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "RecievedAmount",
                table: "SalesInvoices");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Sales",
                type: "datetime(6)",
                nullable: true);
        }
    }
}
