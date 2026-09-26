using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karnel_Travel_Guide.Migrations
{
    /// <inheritdoc />
    public partial class AddTravelIdToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TravelID",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TravelInformationTravelID",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_TravelInformationTravelID",
                table: "Packages",
                column: "TravelInformationTravelID");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_TravelInformations_TravelInformationTravelID",
                table: "Packages",
                column: "TravelInformationTravelID",
                principalTable: "TravelInformations",
                principalColumn: "TravelID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_TravelInformations_TravelInformationTravelID",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_TravelInformationTravelID",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "TravelID",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "TravelInformationTravelID",
                table: "Packages");
        }
    }
}
