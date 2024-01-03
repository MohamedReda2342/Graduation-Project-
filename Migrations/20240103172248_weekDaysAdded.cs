using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    public partial class weekDaysAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumOfDays",
                table: "Medicines");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Medicines",
                newName: "Time4");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Medicines",
                newName: "Time3");

            migrationBuilder.AddColumn<string>(
                name: "EndDate",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Friday",
                table: "Medicines",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Monday",
                table: "Medicines",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Saturday",
                table: "Medicines",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartDate",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Sunday",
                table: "Medicines",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Thursday",
                table: "Medicines",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Time1",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Time2",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Tuesday",
                table: "Medicines",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Wednesday",
                table: "Medicines",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Friday",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Monday",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Saturday",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Sunday",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Thursday",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Time1",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Time2",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Tuesday",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Wednesday",
                table: "Medicines");

            migrationBuilder.RenameColumn(
                name: "Time4",
                table: "Medicines",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "Time3",
                table: "Medicines",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "NumOfDays",
                table: "Medicines",
                type: "int",
                nullable: true);
        }
    }
}
