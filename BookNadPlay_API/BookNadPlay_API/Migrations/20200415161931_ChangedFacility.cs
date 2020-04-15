using Microsoft.EntityFrameworkCore.Migrations;

namespace BookNadPlay_API.Migrations
{
    public partial class ChangedFacility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingNumber",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Facilities");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Facilities",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Facilities",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Facilities");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "BuildingNumber",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
