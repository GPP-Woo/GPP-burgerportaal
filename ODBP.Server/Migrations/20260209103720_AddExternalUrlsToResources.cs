using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ODBP.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalUrlsToResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "A11yUrl",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactUrl",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivacyUrl",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "A11yUrl", "ContactUrl", "PrivacyUrl", "WebsiteUrl" },
                values: new object[] { null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "A11yUrl",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "ContactUrl",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "PrivacyUrl",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "Resources");
        }
    }
}
