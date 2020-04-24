using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookNadPlay_API.Migrations
{
    public partial class AccesPeriodUpdate4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Start",
                table: "AccessPeriods");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "AccessPeriods");

            migrationBuilder.AddColumn<int>(
                name: "EndHour",
                table: "AccessPeriods",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndMinute",
                table: "AccessPeriods",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartHour",
                table: "AccessPeriods",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartMinute",
                table: "AccessPeriods",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndHour",
                table: "AccessPeriods");

            migrationBuilder.DropColumn(
                name: "EndMinute",
                table: "AccessPeriods");

            migrationBuilder.DropColumn(
                name: "StartHour",
                table: "AccessPeriods");

            migrationBuilder.DropColumn(
                name: "StartMinute",
                table: "AccessPeriods");

            migrationBuilder.AddColumn<DateTime>(
                name: "Start",
                table: "AccessPeriods",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "AccessPeriods",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
