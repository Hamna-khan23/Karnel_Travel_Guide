using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karnel_Travel_Guide.Migrations
{
    /// <inheritdoc />
    public partial class FixPackageTravelRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_TravelInformations_TravelInformationTravelID",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_TravelInformationTravelID",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "TravelInformationTravelID",
                table: "Packages");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_TravelID",
                table: "Packages",
                column: "TravelID");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_TravelInformations_TravelID",
                table: "Packages",
                column: "TravelID",
                principalTable: "TravelInformations",
                principalColumn: "TravelID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_TravelInformations_TravelID",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_TravelID",
                table: "Packages");

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
    }
}
