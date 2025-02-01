using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 5, "permissions", "students:read", "0194ba0b-a50d-7568-b187-227d0faed2e9" },
                    { 6, "permissions", "students:courses", "0194ba0b-a50d-7568-b187-227d0faed2e9" },
                    { 7, "permissions", "attendance:read", "0194ba0b-a50d-7568-b187-227d0faed2e9" },
                    { 8, "permissions", "fingerprint:admin", "0194ba0b-a50d-7568-b187-227d0faed2e9" },
                    { 9, "permissions", "fingerprint:match", "0194ba0b-a50d-7568-b187-227d0faed2e9" },
                    { 10, "permissions", "fingerprint:add", "0194ba0b-a50d-7568-b187-227d0faed2e9" },
                    { 11, "permissions", "fingerprint:action", "0194ba0b-a50d-7568-b187-227d0faed2e9" },
                    { 12, "permissions", "fingerprint:register", "0194ba0b-a50d-7568-b187-227d0faed2e9" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}
