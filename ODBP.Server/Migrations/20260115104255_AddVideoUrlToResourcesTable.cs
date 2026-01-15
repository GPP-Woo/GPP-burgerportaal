using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ODBP.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoUrlToResourcesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1,
                column: "VideoUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Resources");
        }
    }
}
