using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsStudentAndNotActiveInstructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0194c663-9838-7852-8811-03b88a11fa8b");

            migrationBuilder.DropColumn(
                name: "IsStudent",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0194ba0b-a50d-7568-b187-2279f6b03b05",
                column: "IsDefault",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStudent",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0194ba0b-a50d-7568-b187-2279f6b03b05",
                column: "IsDefault",
                value: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[] { "0194c663-9838-7852-8811-03b88a11fa8b", "0194c663-9839-7070-aa74-9095f1be5595", true, false, "NotActiveInstructor", "NOTACTIVEINSTRUCTOR" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "E589330B-4F0E-451F-BE37-86519AAFC11A",
                column: "IsStudent",
                value: false);
        }
    }
}
