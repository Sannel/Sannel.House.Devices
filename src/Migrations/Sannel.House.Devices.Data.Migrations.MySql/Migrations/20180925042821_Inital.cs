using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sannel.House.Devices.Data.Migrations.MySql.Migrations
{
    public partial class Inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlternateDeviceIds",
                columns: table => new
                {
                    AlternateId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DeviceId = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Uuid = table.Column<Guid>(nullable: true),
                    MacAddress = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternateDeviceIds", x => x.AlternateId);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    DeviceId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    IsReadOnly = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DeviceId);
                });
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
