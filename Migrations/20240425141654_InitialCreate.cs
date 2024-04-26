using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace summeringsMakker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CaseSummaries",
                columns: table => new
                {
                    CaseSummaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Summary = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MermaidCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseSummaries", x => x.CaseSummaryId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LegalReferences",
                columns: table => new
                {
                    LegalReferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalReferences", x => x.LegalReferenceId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    WordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.WordId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CaseSummaryLegalReferences",
                columns: table => new
                {
                    CaseSummaryId = table.Column<int>(type: "int", nullable: false),
                    LegalReferenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseSummaryLegalReferences", x => new { x.CaseSummaryId, x.LegalReferenceId });
                    table.ForeignKey(
                        name: "FK_CaseSummaryLegalReferences_CaseSummaries_CaseSummaryId",
                        column: x => x.CaseSummaryId,
                        principalTable: "CaseSummaries",
                        principalColumn: "CaseSummaryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseSummaryLegalReferences_LegalReferences_LegalReferenceId",
                        column: x => x.LegalReferenceId,
                        principalTable: "LegalReferences",
                        principalColumn: "LegalReferenceId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CaseSummaryWords",
                columns: table => new
                {
                    CaseSummaryId = table.Column<int>(type: "int", nullable: false),
                    WordId = table.Column<int>(type: "int", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseSummaryWords", x => new { x.CaseSummaryId, x.WordId });
                    table.ForeignKey(
                        name: "FK_CaseSummaryWords_CaseSummaries_CaseSummaryId",
                        column: x => x.CaseSummaryId,
                        principalTable: "CaseSummaries",
                        principalColumn: "CaseSummaryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseSummaryWords_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "WordId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CaseSummaryLegalReferences_LegalReferenceId",
                table: "CaseSummaryLegalReferences",
                column: "LegalReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseSummaryWords_WordId",
                table: "CaseSummaryWords",
                column: "WordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseSummaryLegalReferences");

            migrationBuilder.DropTable(
                name: "CaseSummaryWords");

            migrationBuilder.DropTable(
                name: "LegalReferences");

            migrationBuilder.DropTable(
                name: "CaseSummaries");

            migrationBuilder.DropTable(
                name: "Words");
        }
    }
}
