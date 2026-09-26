using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karnel_Travel_Guide.Migrations
{
    /// <inheritdoc />
    public partial class AddTouristSpotAndRestaurantT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RestaurantID",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TouristSpotID",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RestaurantID",
                table: "Bookings",
                column: "RestaurantID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TouristSpotID",
                table: "Bookings",
                column: "TouristSpotID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Restaurants_RestaurantID",
                table: "Bookings",
                column: "RestaurantID",
                principalTable: "Restaurants",
                principalColumn: "RestaurantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TouristSpots_TouristSpotID",
                table: "Bookings",
                column: "TouristSpotID",
                principalTable: "TouristSpots",
                principalColumn: "SpotID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Restaurants_RestaurantID",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TouristSpots_TouristSpotID",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RestaurantID",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TouristSpotID",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RestaurantID",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TouristSpotID",
                table: "Bookings");
        }
    }
}
