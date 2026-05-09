using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Attendance_GPS.Migrations
{
    /// <inheritdoc />
    public partial class AddFaceRecognitionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "FaceEmbedding",
                table: "Users",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FaceEnrolledAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFaceVerificationEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaceEmbedding",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FaceEnrolledAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsFaceVerificationEnabled",
                table: "Users");
        }
    }
}
