using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class saleReturnAmountAdjust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "SalesReturnItems");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "SalesReturnInvoice",
                newName: "TaxableAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaxableAmount",
                table: "SalesReturnInvoice",
                newName: "DiscountAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "SalesReturnItems",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
