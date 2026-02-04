using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ODBP.Migrations
{
    /// <inheritdoc />
    public partial class AddAfbeeldingenToResourcesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaviconFileName",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoFileName",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FaviconFileName", "ImageFileName", "LogoFileName" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaviconFileName",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "LogoFileName",
                table: "Resources");
        }
    }
}
