using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    public partial class a4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthMeasurements");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.AddColumn<double>(
                name: "HeartRate",
                table: "Patients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Patients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Patients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "O2",
                table: "Patients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "Patients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeartRate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "O2",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Patients");

            migrationBuilder.CreateTable(
                name: "HealthMeasurements",
                columns: table => new
                {
                    HealthMeasurementsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    HeartRate = table.Column<double>(type: "float", nullable: false),
                    O2 = table.Column<double>(type: "float", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthMeasurements", x => x.HealthMeasurementsId);
                    table.ForeignKey(
                        name: "FK_HealthMeasurements_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Locations_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthMeasurements_PatientId",
                table: "HealthMeasurements",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_PatientId",
                table: "Locations",
                column: "PatientId",
                unique: true);
        }
    }
}
