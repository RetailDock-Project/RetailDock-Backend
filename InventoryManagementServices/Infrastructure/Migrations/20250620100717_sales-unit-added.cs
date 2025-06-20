using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class salesunitadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnItems_UnitId",
                table: "SalesReturnItems",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_UnitId",
                table: "SaleItems",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_UnitOfMeasures_UnitId",
                table: "SaleItems",
                column: "UnitId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesReturnItems_UnitOfMeasures_UnitId",
                table: "SalesReturnItems",
                column: "UnitId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_UnitOfMeasures_UnitId",
                table: "SaleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesReturnItems_UnitOfMeasures_UnitId",
                table: "SalesReturnItems");

            migrationBuilder.DropIndex(
                name: "IX_SalesReturnItems_UnitId",
                table: "SalesReturnItems");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_UnitId",
                table: "SaleItems");
        }
    }
}
