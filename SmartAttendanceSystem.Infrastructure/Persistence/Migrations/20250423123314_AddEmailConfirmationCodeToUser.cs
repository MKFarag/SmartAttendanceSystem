using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailConfirmationCodeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailConfirmationCodeExpiration",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "E589330B-4F0E-451F-BE37-86519AAFC11A",
                columns: new[] { "EmailConfirmationCode", "EmailConfirmationCodeExpiration" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationCodeExpiration",
                table: "AspNetUsers");
        }
    }
}
