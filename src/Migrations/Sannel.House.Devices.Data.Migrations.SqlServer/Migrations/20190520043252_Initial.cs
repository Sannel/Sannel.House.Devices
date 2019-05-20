using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sannel.House.Devices.Data.Migrations.SqlServer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    DeviceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false),
                    IsReadOnly = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DeviceId);
                });

            migrationBuilder.CreateTable(
                name: "AlternateDeviceIds",
                columns: table => new
                {
                    AlternateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceId = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(nullable: false),
                    Uuid = table.Column<Guid>(nullable: true),
                    MacAddress = table.Column<long>(nullable: true),
                    Manufacture = table.Column<string>(nullable: true),
                    ManufactureId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternateDeviceIds", x => x.AlternateId);
                    table.ForeignKey(
                        name: "FK_AlternateDeviceIds_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlternateDeviceIds_DeviceId",
                table: "AlternateDeviceIds",
                column: "DeviceId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DisplayOrder",
                table: "Devices",
                column: "DisplayOrder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlternateDeviceIds");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
