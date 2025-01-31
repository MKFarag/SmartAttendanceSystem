using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyOnRoleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0194ba0b-a50d-7568-b187-2279f6b03b05",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Student", "STUDENT" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[] { "0194bd46-ceca-7ea4-9d02-9e268a031756", "0194bd47-1793-75b0-a10b-9de81d580d84", false, false, "Instructor", "INSTRUCTOR" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "E589330B-4F0E-451F-BE37-86519AAFC11A",
                column: "IsStudent",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0194bd46-ceca-7ea4-9d02-9e268a031756");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0194ba0b-a50d-7568-b187-2279f6b03b05",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Member", "MEMBER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "E589330B-4F0E-451F-BE37-86519AAFC11A",
                column: "IsStudent",
                value: true);
        }
    }
}
