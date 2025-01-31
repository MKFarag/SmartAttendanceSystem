using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0194ba0b-a50d-7568-b187-2279f6b03b05", "0194ba0b-a50d-7568-b187-227aba8bc12f", true, false, "Member", "MEMBER" },
                    { "0194ba0b-a50d-7568-b187-227d0faed2e9", "0194ba0b-a50d-7568-b187-227ea7268643", false, false, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "IsStudent", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "E589330B-4F0E-451F-BE37-86519AAFC11A", 0, "0194b8e5-7a29-7829-925e-f718a0ca8ba2", "Admin@SAS.com", true, true, false, null, "SAS Admin", "ADMIN@SAS.COM", "ADMIN@SAS.COM", "AQAAAAIAAYagAAAAEFaOdqUF6MPmAVtOw2LQ65EhwUw6gjsiHamXzDnwfLMBf+cg5DgcLGQm7aaFmjkgQw==", null, false, "F15E8DE749FC4ABB817E6F6B54EA4ADB", false, "Admin@SAS.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "courses:read", "0194ba0b-a50d-7568-b187-2279f6b03b05" },
                    { 2, "permissions", "courses:modify", "0194ba0b-a50d-7568-b187-2279f6b03b05" },
                    { 3, "permissions", "departments:read", "0194ba0b-a50d-7568-b187-2279f6b03b05" },
                    { 4, "permissions", "departments:modify", "0194ba0b-a50d-7568-b187-2279f6b03b05" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "0194ba0b-a50d-7568-b187-227d0faed2e9", "E589330B-4F0E-451F-BE37-86519AAFC11A" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0194ba0b-a50d-7568-b187-227d0faed2e9", "E589330B-4F0E-451F-BE37-86519AAFC11A" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0194ba0b-a50d-7568-b187-2279f6b03b05");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0194ba0b-a50d-7568-b187-227d0faed2e9");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "E589330B-4F0E-451F-BE37-86519AAFC11A");
        }
    }
}
