using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_HsnCodes_HsnCodeNumber",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "HsnCodeNumber",
                table: "Products",
                newName: "HsnCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_HsnCodeNumber",
                table: "Products",
                newName: "IX_Products_HsnCodeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "HsnCodes",
                newName: "HsnCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_HsnCodes_HsnCodeId",
                table: "Products",
                column: "HsnCodeId",
                principalTable: "HsnCodes",
                principalColumn: "HsnCodeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_HsnCodes_HsnCodeId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "HsnCodeId",
                table: "Products",
                newName: "HsnCodeNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Products_HsnCodeId",
                table: "Products",
                newName: "IX_Products_HsnCodeNumber");

            migrationBuilder.RenameColumn(
                name: "HsnCodeId",
                table: "HsnCodes",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_HsnCodes_HsnCodeNumber",
                table: "Products",
                column: "HsnCodeNumber",
                principalTable: "HsnCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
