using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_StudentId_CourseId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId_CourseId",
                table: "Attendances",
                columns: new[] { "StudentId", "CourseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_StudentId_CourseId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId_CourseId",
                table: "Attendances",
                columns: new[] { "StudentId", "CourseId" });
        }
    }
}
