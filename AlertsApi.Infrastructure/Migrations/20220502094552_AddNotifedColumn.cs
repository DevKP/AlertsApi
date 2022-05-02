using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertsApi.Infrastructure.Migrations
{
    public partial class AddNotifedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsersNotified",
                table: "Alerts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsersNotified",
                table: "Alerts");
        }
    }
}
