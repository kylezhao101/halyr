using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Halyr.Api.Migrations
{
    /// <inheritdoc />
    public partial class storeEnvironmentAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Environment",
                table: "FeatureFlagEnvironments",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Environment",
                table: "FeatureFlagEnvironments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
