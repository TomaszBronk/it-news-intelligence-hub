using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItNewsIntelligenceHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsItemNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "NewsItems",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "NewsItems");
        }
    }
}
