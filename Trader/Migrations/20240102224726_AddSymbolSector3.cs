using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trader.Migrations
{
    /// <inheritdoc />
    public partial class AddSymbolSector3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectorName",
                table: "SymbolSectors");

            migrationBuilder.AddColumn<int>(
                name: "Sector",
                table: "SymbolSectors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 1,
                column: "Sector",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 2,
                column: "Sector",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 3,
                column: "Sector",
                value: 2);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 4,
                column: "Sector",
                value: 3);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 5,
                column: "Sector",
                value: 4);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 6,
                column: "Sector",
                value: 4);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 7,
                column: "Sector",
                value: 5);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 8,
                column: "Sector",
                value: 5);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 9,
                column: "Sector",
                value: 6);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 10,
                column: "Sector",
                value: 5);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 11,
                column: "Sector",
                value: 2);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 12,
                column: "Sector",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 13,
                column: "Sector",
                value: 3);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 14,
                column: "Sector",
                value: 3);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 15,
                column: "Sector",
                value: 7);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 16,
                column: "Sector",
                value: 7);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 17,
                column: "Sector",
                value: 3);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 18,
                column: "Sector",
                value: 2);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 19,
                column: "Sector",
                value: 2);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 20,
                column: "Sector",
                value: 2);

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 21,
                column: "Sector",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sector",
                table: "SymbolSectors");

            migrationBuilder.AddColumn<string>(
                name: "SectorName",
                table: "SymbolSectors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 1,
                column: "SectorName",
                value: "Energy");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 2,
                column: "SectorName",
                value: "Energy");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 3,
                column: "SectorName",
                value: "Consumer Defensive");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 4,
                column: "SectorName",
                value: "Consumer cyclical");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 5,
                column: "SectorName",
                value: "Communication services");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 6,
                column: "SectorName",
                value: "Communication services");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 7,
                column: "SectorName",
                value: "Healthcare");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 8,
                column: "SectorName",
                value: "Healthcare");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 9,
                column: "SectorName",
                value: "Technology");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 10,
                column: "SectorName",
                value: "Healthcare");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 11,
                column: "SectorName",
                value: "Consumer Defensive");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 12,
                column: "SectorName",
                value: "Energy");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 13,
                column: "SectorName",
                value: "Consumer cyclical");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 14,
                column: "SectorName",
                value: "Consumer cyclical");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 15,
                column: "SectorName",
                value: "Financial");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 16,
                column: "SectorName",
                value: "Financial");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 17,
                column: "SectorName",
                value: "Consumer cyclical");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 18,
                column: "SectorName",
                value: "Consumer Defensive");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 19,
                column: "SectorName",
                value: "Consumer Defensive");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 20,
                column: "SectorName",
                value: "Consumer Defensive");

            migrationBuilder.UpdateData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 21,
                column: "SectorName",
                value: "Consumer Defensive");
        }
    }
}
