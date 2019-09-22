using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace thermo_scrape.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThermostatLogs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Json = table.Column<string>(nullable: true),
                    Stamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThermostatLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThermostatLogs");
        }
    }
}
