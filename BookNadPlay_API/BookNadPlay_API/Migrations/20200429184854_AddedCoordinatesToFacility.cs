using Microsoft.EntityFrameworkCore.Migrations;

namespace BookNadPlay_API.Migrations
{
    public partial class AddedCoordinatesToFacility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Facilities",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "Facilities",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Lon",
                table: "Facilities",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Lon",
                table: "Facilities");
        }
    }
}
