using Microsoft.EntityFrameworkCore.Migrations;

namespace BookNadPlay_API.Migrations
{
    public partial class AddedPhoneNumberToFacility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Facilities",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Facilities");
        }
    }
}
