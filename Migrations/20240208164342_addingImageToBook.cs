using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksSpring2024.Migrations
{
    /// <inheritdoc />
    public partial class addingImageToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImgURL",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 1,
                column: "ImgURL",
                value: "");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 2,
                column: "ImgURL",
                value: "");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 3,
                column: "ImgURL",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgURL",
                table: "Books");
        }
    }
}
