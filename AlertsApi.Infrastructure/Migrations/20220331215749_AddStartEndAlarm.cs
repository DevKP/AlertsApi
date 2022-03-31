using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertsApi.Infrastructure.Migrations
{
    public partial class AddStartEndAlarm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Alerts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Alerts",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Alerts");
        }
    }
}
