using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 22, "permissions", "courses:read", "0194bd46-ceca-7ea4-9d02-9e268a031756" },
                    { 23, "permissions", "departments:read", "0194bd46-ceca-7ea4-9d02-9e268a031756" },
                    { 24, "permissions", "students:read", "0194bd46-ceca-7ea4-9d02-9e268a031756" },
                    { 25, "permissions", "students:create", "0194bd46-ceca-7ea4-9d02-9e268a031756" },
                    { 26, "permissions", "attendance:read", "0194bd46-ceca-7ea4-9d02-9e268a031756" },
                    { 27, "permissions", "fingerprint:match", "0194bd46-ceca-7ea4-9d02-9e268a031756" },
                    { 28, "permissions", "fingerprint:add", "0194bd46-ceca-7ea4-9d02-9e268a031756" },
                    { 29, "permissions", "fingerprint:action", "0194bd46-ceca-7ea4-9d02-9e268a031756" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 29);
        }
    }
}
