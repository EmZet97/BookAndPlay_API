using Microsoft.EntityFrameworkCore.Migrations;

namespace BookNadPlay_API.Migrations
{
    public partial class Accesperiodaddedfacilityid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessPeriods_Facilities_FacilityId",
                table: "AccessPeriods");

            migrationBuilder.AlterColumn<int>(
                name: "FacilityId",
                table: "AccessPeriods",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessPeriods_Facilities_FacilityId",
                table: "AccessPeriods",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "FacilityId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessPeriods_Facilities_FacilityId",
                table: "AccessPeriods");

            migrationBuilder.AlterColumn<int>(
                name: "FacilityId",
                table: "AccessPeriods",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_AccessPeriods_Facilities_FacilityId",
                table: "AccessPeriods",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "FacilityId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
