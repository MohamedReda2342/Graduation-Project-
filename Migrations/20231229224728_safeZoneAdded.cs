using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    public partial class safeZoneAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Radius",
                table: "Patients",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SafeZoneLatitude",
                table: "Patients",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SafeZoneLongitude",
                table: "Patients",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Radius",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "SafeZoneLatitude",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "SafeZoneLongitude",
                table: "Patients");
        }
    }
}
