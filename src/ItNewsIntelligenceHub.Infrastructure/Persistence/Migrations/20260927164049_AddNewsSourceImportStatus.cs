using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ItNewsIntelligenceHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsSourceImportStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastFetchAttemptAtUtc",
                table: "NewsSources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastFetchError",
                table: "NewsSources",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSuccessfulFetchAtUtc",
                table: "NewsSources",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastFetchAttemptAtUtc",
                table: "NewsSources");

            migrationBuilder.DropColumn(
                name: "LastFetchError",
                table: "NewsSources");

            migrationBuilder.DropColumn(
                name: "LastSuccessfulFetchAtUtc",
                table: "NewsSources");
        }
    }
}
