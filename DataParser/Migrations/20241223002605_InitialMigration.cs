using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataParser.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "passport_series",
                columns: table => new
                {
                    series_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    series = table.Column<short>(type: "smallint", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passport_series", x => x.series_id);
                });

            migrationBuilder.CreateTable(
                name: "passport_numbers",
                columns: table => new
                {
                    number_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    series_id = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passport_numbers", x => x.number_id);
                    table.ForeignKey(
                        name: "FK_passport_numbers_passport_series_series_id",
                        column: x => x.series_id,
                        principalTable: "passport_series",
                        principalColumn: "series_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "changelog",
                columns: table => new
                {
                    change_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number_id = table.Column<int>(type: "integer", nullable: false),
                    date_of_change = table.Column<int>(type: "int", nullable: false),
                    change_status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_changelog", x => x.change_id);
                    table.ForeignKey(
                        name: "FK_changelog_passport_numbers_number_id",
                        column: x => x.number_id,
                        principalTable: "passport_numbers",
                        principalColumn: "number_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_changelog_number_id",
                table: "changelog",
                column: "number_id");

            migrationBuilder.CreateIndex(
                name: "IX_passport_numbers_series_id",
                table: "passport_numbers",
                column: "series_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "changelog");

            migrationBuilder.DropTable(
                name: "passport_numbers");

            migrationBuilder.DropTable(
                name: "passport_series");
        }
    }
}
