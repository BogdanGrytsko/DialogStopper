using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trader.Migrations
{
    /// <inheritdoc />
    public partial class EOG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SymbolSectors",
                columns: new[] { "Id", "Sector", "Symbol" },
                values: new object[] { 22, 1, "EOG" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SymbolSectors",
                keyColumn: "Id",
                keyValue: 22);
        }
    }
}
