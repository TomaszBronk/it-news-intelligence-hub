using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItNewsIntelligenceHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsItemStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "NewsItems",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "NewsItems");
        }
    }
}
