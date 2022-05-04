using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertsApi.Infrastructure.Migrations
{
    public partial class AddForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Alerts_AlertLocationName",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<string>(
                name: "AlertLocationName",
                table: "Subscriptions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Alerts_AlertLocationName",
                table: "Subscriptions",
                column: "AlertLocationName",
                principalTable: "Alerts",
                principalColumn: "LocationName",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Alerts_AlertLocationName",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<string>(
                name: "AlertLocationName",
                table: "Subscriptions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Alerts_AlertLocationName",
                table: "Subscriptions",
                column: "AlertLocationName",
                principalTable: "Alerts",
                principalColumn: "LocationName");
        }
    }
}
