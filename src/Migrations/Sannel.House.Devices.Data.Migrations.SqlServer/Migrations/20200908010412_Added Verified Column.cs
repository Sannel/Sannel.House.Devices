using Microsoft.EntityFrameworkCore.Migrations;

namespace Sannel.House.Devices.Data.Migrations.SqlServer.Migrations
{
    public partial class AddedVerifiedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "Devices",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Verified",
                table: "Devices");
        }
    }
}
