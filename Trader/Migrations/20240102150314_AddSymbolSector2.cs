using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Trader.Migrations
{
    /// <inheritdoc />
    public partial class AddSymbolSector2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SymbolSectors",
                columns: new[] { "Id", "SectorName", "Symbol" },
                values: new object[,]
                {
                    { 12, "Energy", "COP" },
                    { 13, "Consumer cyclical", "F" },
                    { 14, "Consumer cyclical", "HD" },
                    { 15, "Financial", "JPM" },
                    { 16, "Financial", "BAC" },
                    { 17, "Consumer cyclical", "SBUX" },
                    { 18, "Consumer Defensive", "PG" },
                    { 19, "Consumer Defensive", "CL" },
                    { 20, "Consumer Defensive", "PEP" },
                    { 21, "Consumer Defensive", "PM" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 21);
        }
    }
}
