using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertsApi.Infrastructure.Migrations
{
    public partial class MakeHashTagPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Alerts_AlertLocationName",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Alerts",
                table: "Alerts");

            migrationBuilder.RenameColumn(
                name: "AlertLocationName",
                table: "Subscriptions",
                newName: "AlertHashTag");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_AlertLocationName",
                table: "Subscriptions",
                newName: "IX_Subscriptions_AlertHashTag");

            migrationBuilder.AlterColumn<string>(
                name: "LocationName",
                table: "Alerts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "LocationHashTag",
                table: "Alerts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Alerts",
                table: "Alerts",
                column: "LocationHashTag");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Alerts_AlertHashTag",
                table: "Subscriptions",
                column: "AlertHashTag",
                principalTable: "Alerts",
                principalColumn: "LocationHashTag",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Alerts_AlertHashTag",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Alerts",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "LocationHashTag",
                table: "Alerts");

            migrationBuilder.RenameColumn(
                name: "AlertHashTag",
                table: "Subscriptions",
                newName: "AlertLocationName");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_AlertHashTag",
                table: "Subscriptions",
                newName: "IX_Subscriptions_AlertLocationName");

            migrationBuilder.AlterColumn<string>(
                name: "LocationName",
                table: "Alerts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Alerts",
                table: "Alerts",
                column: "LocationName");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Alerts_AlertLocationName",
                table: "Subscriptions",
                column: "AlertLocationName",
                principalTable: "Alerts",
                principalColumn: "LocationName",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
