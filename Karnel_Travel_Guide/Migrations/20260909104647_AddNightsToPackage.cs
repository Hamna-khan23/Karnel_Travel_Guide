using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karnel_Travel_Guide.Migrations
{
    /// <inheritdoc />
    public partial class AddNightsToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Nights",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nights",
                table: "Packages");
        }
    }
}
