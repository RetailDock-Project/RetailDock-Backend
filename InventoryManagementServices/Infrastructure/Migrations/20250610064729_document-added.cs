using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class documentadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Purchases");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Purchases",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "Purchases",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "LedgerId",
                table: "Purchases",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "SupplierInvoiceNumber",
                table: "Purchases",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "CGST",
                table: "PurchaseItems",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IGST",
                table: "PurchaseItems",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SGST",
                table: "PurchaseItems",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "PurchaseItems",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UGST",
                table: "PurchaseItems",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CGST",
                table: "PurchaseInvoices",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GstType",
                table: "PurchaseInvoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "IGST",
                table: "PurchaseInvoices",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SGST",
                table: "PurchaseInvoices",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UGST",
                table: "PurchaseInvoices",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DocumentData = table.Column<byte[]>(type: "MEDIUMBLOB", nullable: false),
                    FileName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileNote = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContentType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_DocumentId",
                table: "Purchases",
                column: "DocumentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Documents_DocumentId",
                table: "Purchases",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Documents_DocumentId",
                table: "Purchases");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_DocumentId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "LedgerId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "SupplierInvoiceNumber",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CGST",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "IGST",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "SGST",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "UGST",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "CGST",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "GstType",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "IGST",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "SGST",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "UGST",
                table: "PurchaseInvoices");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Purchases",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
