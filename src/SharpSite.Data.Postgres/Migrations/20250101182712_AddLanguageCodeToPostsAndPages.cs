using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharpSite.Data.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageCodeToPostsAndPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "Posts",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "Pages",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            // Update existing rows with default value "en"
            migrationBuilder.Sql("UPDATE \"Posts\" SET \"LanguageCode\" = 'en' WHERE \"LanguageCode\" = ''");
            migrationBuilder.Sql("UPDATE \"Pages\" SET \"LanguageCode\" = 'en' WHERE \"LanguageCode\" = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "Pages");
        }
    }
}
