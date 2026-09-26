using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karnel_Travel_Guide.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelID",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_HotelID",
                table: "Packages",
                column: "HotelID");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Hotels_HotelID",
                table: "Packages",
                column: "HotelID",
                principalTable: "Hotels",
                principalColumn: "HotelID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Hotels_HotelID",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_HotelID",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "HotelID",
                table: "Packages");
        }
    }
}
