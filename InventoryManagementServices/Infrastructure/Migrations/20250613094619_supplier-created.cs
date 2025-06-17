using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class suppliercreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "LedgerId",
            //    table: "Purchases",
            //    newName: "SupplierId");

            //migrationBuilder.RenameColumn(
            //    name: "LedgerId",
            //    table: "PurchaseReturns",
            //    newName: "SupplierId");

            //migrationBuilder.CreateTable(
            //    name: "Suppliers",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //        OrganizationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //        Name = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Email = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Address = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        City = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        State = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Country = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Pincode = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        GSTNumber = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
            //        CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
            //        CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Suppliers", x => x.Id);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Purchases_SupplierId",
            //    table: "Purchases",
            //    column: "SupplierId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PurchaseReturns_SupplierId",
            //    table: "PurchaseReturns",
            //    column: "SupplierId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_PurchaseReturns_Suppliers_SupplierId",
            //    table: "PurchaseReturns",
            //    column: "SupplierId",
            //    principalTable: "Suppliers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Purchases_Suppliers_SupplierId",
            //    table: "Purchases",
            //    column: "SupplierId",
            //    principalTable: "Suppliers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_PurchaseReturns_Suppliers_SupplierId",
            //    table: "PurchaseReturns");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Purchases_Suppliers_SupplierId",
            //    table: "Purchases");

            //migrationBuilder.DropTable(
            //    name: "Suppliers");

            //migrationBuilder.DropIndex(
            //    name: "IX_Purchases_SupplierId",
            //    table: "Purchases");

            //migrationBuilder.DropIndex(
            //    name: "IX_PurchaseReturns_SupplierId",
            //    table: "PurchaseReturns");

            //migrationBuilder.RenameColumn(
            //    name: "SupplierId",
            //    table: "Purchases",
            //    newName: "LedgerId");

            //migrationBuilder.RenameColumn(
            //    name: "SupplierId",
            //    table: "PurchaseReturns",
            //    newName: "LedgerId");
        }
    }
}
