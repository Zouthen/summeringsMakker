using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace summeringsMakker.Migrations
{
    /// <inheritdoc />
    public partial class updated_ids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaseId",
                table: "CaseSummaries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseId",
                table: "CaseSummaries");
        }
    }
}
