using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Halyr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFlagDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FeatureFlags",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "FeatureFlags");
        }
    }
}
