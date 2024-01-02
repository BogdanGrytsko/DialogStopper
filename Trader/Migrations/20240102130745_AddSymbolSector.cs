using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Trader.Migrations
{
    /// <inheritdoc />
    public partial class AddSymbolSector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SymbolSectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    SectorName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymbolSectors", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SymbolSectors",
                columns: new[] { "Id", "SectorName", "Symbol" },
                values: new object[,]
                {
                    { 1, "Energy", "XOM" },
                    { 2, "Energy", "CVX" },
                    { 3, "Consumer Defensive", "KO" },
                    { 4, "Consumer cyclical", "MCD" },
                    { 5, "Communication services", "T" },
                    { 6, "Communication services", "VZ" },
                    { 7, "Healthcare", "JNJ" },
                    { 8, "Healthcare", "PFE" },
                    { 9, "Technology", "IBM" },
                    { 10, "Healthcare", "ABBV" },
                    { 11, "Consumer Defensive", "TGT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SymbolSectors_Symbol",
                table: "SymbolSectors",
                column: "Symbol",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SymbolSectors");
        }
    }
}
