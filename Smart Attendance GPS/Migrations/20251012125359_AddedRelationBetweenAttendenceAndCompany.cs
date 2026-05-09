using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Attendance_GPS.Migrations
{
    /// <inheritdoc />
    public partial class AddedRelationBetweenAttendenceAndCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Attendances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_CompanyId",
                table: "Attendances",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Companies_CompanyId",
                table: "Attendances",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Companies_CompanyId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_CompanyId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Attendances");
        }
    }
}
