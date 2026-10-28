using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ODBP.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsToResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnalyticsIdentifier",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnalyticsTool",
                table: "Resources",
                type: "text",
                nullable: false,
                defaultValue: "None");

            migrationBuilder.AddColumn<string>(
                name: "AnalyticsUrl",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AnalyticsIdentifier", "AnalyticsUrl" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnalyticsIdentifier",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "AnalyticsTool",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "AnalyticsUrl",
                table: "Resources");
        }
    }
}
