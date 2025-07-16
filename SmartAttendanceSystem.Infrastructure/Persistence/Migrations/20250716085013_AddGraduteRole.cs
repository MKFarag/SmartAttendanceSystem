using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGraduteRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDisabled", "Name", "NormalizedName" },
                values: new object[] { "01981255-1cb9-7997-ac73-b1213daf4a8a", "01981255-526f-71af-b4cb-492dc3aef871", false, false, "Graduate", "GRADUATE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "01981255-1cb9-7997-ac73-b1213daf4a8a");
        }
    }
}
