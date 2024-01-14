using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Trader.Migrations
{
    /// <inheritdoc />
    public partial class energia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SymbolSectors",
                columns: new[] { "Id", "Sector", "Symbol" },
                values: new object[,]
                {
                    { 23, 1, "PXD" },
                    { 24, 1, "FANG" },
                    { 25, 1, "PSX" },
                    { 26, 1, "VLO" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 26);
        }
    }
}
