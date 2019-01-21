using Microsoft.EntityFrameworkCore.Migrations;

namespace Sannel.House.Devices.Data.Migrations.Sqlite.Migrations
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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlternateDeviceIds_Uuid",
                table: "AlternateDeviceIds",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlternateDeviceIds_Manufacture_ManufactureId",
                table: "AlternateDeviceIds",
                columns: new[] { "Manufacture", "ManufactureId" },
                unique: true);
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

            /*migrationBuilder.DropColumn(
                name: "Manufacture",
                table: "AlternateDeviceIds");

            migrationBuilder.DropColumn(
                name: "ManufactureId",
                table: "AlternateDeviceIds");*/
        }
    }
}
