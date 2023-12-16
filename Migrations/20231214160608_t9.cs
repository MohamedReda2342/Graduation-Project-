using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    public partial class t9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_Users_UserId",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_UserId",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Medicines");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Medicines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_UserId",
                table: "Medicines",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_Users_UserId",
                table: "Medicines",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
