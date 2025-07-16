using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUpgradeStudentPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7,
                column: "ClaimValue",
                value: "students:upgrade");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8,
                column: "ClaimValue",
                value: "students:courses");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9,
                column: "ClaimValue",
                value: "attendance:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10,
                column: "ClaimValue",
                value: "attendance:remove");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11,
                column: "ClaimValue",
                value: "fingerprint:admin");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12,
                column: "ClaimValue",
                value: "fingerprint:match");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13,
                column: "ClaimValue",
                value: "fingerprint:add");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14,
                column: "ClaimValue",
                value: "fingerprint:action");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 15,
                column: "ClaimValue",
                value: "fingerprint:register");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 16,
                column: "ClaimValue",
                value: "Role:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 17,
                column: "ClaimValue",
                value: "Role:add");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 18,
                column: "ClaimValue",
                value: "Role:update");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 19,
                column: "ClaimValue",
                value: "user:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 20,
                column: "ClaimValue",
                value: "user:add");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 21,
                column: "ClaimValue",
                value: "user:update");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 22,
                column: "ClaimValue",
                value: "user:toggle-status");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "user:unlock", "0194ba0b-a50d-7568-b187-227d0faed2e9" });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 24,
                column: "ClaimValue",
                value: "courses:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 25,
                column: "ClaimValue",
                value: "departments:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 26,
                column: "ClaimValue",
                value: "students:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 27,
                column: "ClaimValue",
                value: "students:create");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 28,
                column: "ClaimValue",
                value: "attendance:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 29,
                column: "ClaimValue",
                value: "attendance:remove");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 30,
                column: "ClaimValue",
                value: "fingerprint:match");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 31,
                column: "ClaimValue",
                value: "fingerprint:add");

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[] { 32, "permissions", "fingerprint:action", "0194bd46-ceca-7ea4-9d02-9e268a031756" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7,
                column: "ClaimValue",
                value: "students:courses");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8,
                column: "ClaimValue",
                value: "attendance:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9,
                column: "ClaimValue",
                value: "attendance:remove");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10,
                column: "ClaimValue",
                value: "fingerprint:admin");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11,
                column: "ClaimValue",
                value: "fingerprint:match");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12,
                column: "ClaimValue",
                value: "fingerprint:add");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13,
                column: "ClaimValue",
                value: "fingerprint:action");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14,
                column: "ClaimValue",
                value: "fingerprint:register");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 15,
                column: "ClaimValue",
                value: "Role:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 16,
                column: "ClaimValue",
                value: "Role:add");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 17,
                column: "ClaimValue",
                value: "Role:update");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 18,
                column: "ClaimValue",
                value: "user:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 19,
                column: "ClaimValue",
                value: "user:add");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 20,
                column: "ClaimValue",
                value: "user:update");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 21,
                column: "ClaimValue",
                value: "user:toggle-status");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 22,
                column: "ClaimValue",
                value: "user:unlock");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "courses:read", "0194bd46-ceca-7ea4-9d02-9e268a031756" });

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 24,
                column: "ClaimValue",
                value: "departments:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 25,
                column: "ClaimValue",
                value: "students:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 26,
                column: "ClaimValue",
                value: "students:create");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 27,
                column: "ClaimValue",
                value: "attendance:read");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 28,
                column: "ClaimValue",
                value: "attendance:remove");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 29,
                column: "ClaimValue",
                value: "fingerprint:match");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 30,
                column: "ClaimValue",
                value: "fingerprint:add");

            migrationBuilder.UpdateData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 31,
                column: "ClaimValue",
                value: "fingerprint:action");
        }
    }
}
