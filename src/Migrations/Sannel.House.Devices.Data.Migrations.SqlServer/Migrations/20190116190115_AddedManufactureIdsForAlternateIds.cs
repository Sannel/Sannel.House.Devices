using Microsoft.EntityFrameworkCore.Migrations;

namespace Sannel.House.Devices.Data.Migrations.SqlServer.Migrations
{
    public partial class AddedManufactureIdsForAlternateIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Manufacture",
                table: "AlternateDeviceIds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManufactureId",
                table: "AlternateDeviceIds",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DisplayOrder",
                table: "Devices",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_AlternateDeviceIds_MacAddress",
                table: "AlternateDeviceIds",
                column: "MacAddress",
                unique: true,
                filter: "[MacAddress] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AlternateDeviceIds_Uuid",
                table: "AlternateDeviceIds",
                column: "Uuid",
                unique: true,
                filter: "[Uuid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AlternateDeviceIds_Manufacture_ManufactureId",
                table: "AlternateDeviceIds",
                columns: new[] { "Manufacture", "ManufactureId" },
                unique: true,
                filter: "[Manufacture] IS NOT NULL AND [ManufactureId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Devices_DisplayOrder",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_AlternateDeviceIds_MacAddress",
                table: "AlternateDeviceIds");

            migrationBuilder.DropIndex(
                name: "IX_AlternateDeviceIds_Uuid",
                table: "AlternateDeviceIds");

            migrationBuilder.DropIndex(
                name: "IX_AlternateDeviceIds_Manufacture_ManufactureId",
                table: "AlternateDeviceIds");

            migrationBuilder.DropColumn(
                name: "Manufacture",
                table: "AlternateDeviceIds");

            migrationBuilder.DropColumn(
                name: "ManufactureId",
                table: "AlternateDeviceIds");
        }
    }
}
