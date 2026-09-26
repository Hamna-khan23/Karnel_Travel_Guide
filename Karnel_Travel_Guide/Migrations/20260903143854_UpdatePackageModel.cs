using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karnel_Travel_Guide.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePackageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Packages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Packages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exclusions",
                table: "Packages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inclusions",
                table: "Packages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Packages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ResortID",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RestaurantID",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Packages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalPersons",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ResortID",
                table: "Packages",
                column: "ResortID");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_RestaurantID",
                table: "Packages",
                column: "RestaurantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Resorts_ResortID",
                table: "Packages",
                column: "ResortID",
                principalTable: "Resorts",
                principalColumn: "ResortID");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Restaurants_RestaurantID",
                table: "Packages",
                column: "RestaurantID",
                principalTable: "Restaurants",
                principalColumn: "RestaurantID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Resorts_ResortID",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Restaurants_RestaurantID",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_ResortID",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_RestaurantID",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Exclusions",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Inclusions",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "ResortID",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "RestaurantID",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "TotalPersons",
                table: "Packages");
        }
    }
}
