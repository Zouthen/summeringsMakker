using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace summeringsMakker.Migrations
{
    /// <inheritdoc />
    public partial class LegalReference_bools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActual",
                table: "LegalReferences",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInEffect",
                table: "LegalReferences",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActual",
                table: "LegalReferences");

            migrationBuilder.DropColumn(
                name: "IsInEffect",
                table: "LegalReferences");
        }
    }
}
