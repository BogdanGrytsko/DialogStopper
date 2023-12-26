using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trader.Migrations
{
    /// <inheritdoc />
    public partial class Contraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Dividends_Symbol_ExDate",
                table: "Dividends",
                columns: new[] { "Symbol", "ExDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candles_Symbol_Date",
                table: "Candles",
                columns: new[] { "Symbol", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dividends_Symbol_ExDate",
                table: "Dividends");

            migrationBuilder.DropIndex(
                name: "IX_Candles_Symbol_Date",
                table: "Candles");
        }
    }
}
