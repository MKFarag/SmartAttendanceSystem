using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendances_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Weeks",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "int", nullable: false),
                    Week1 = table.Column<bool>(type: "bit", nullable: true),
                    Week2 = table.Column<bool>(type: "bit", nullable: true),
                    Week3 = table.Column<bool>(type: "bit", nullable: true),
                    Week4 = table.Column<bool>(type: "bit", nullable: true),
                    Week5 = table.Column<bool>(type: "bit", nullable: true),
                    Week6 = table.Column<bool>(type: "bit", nullable: true),
                    Week7 = table.Column<bool>(type: "bit", nullable: true),
                    Week8 = table.Column<bool>(type: "bit", nullable: true),
                    Week9 = table.Column<bool>(type: "bit", nullable: true),
                    Week10 = table.Column<bool>(type: "bit", nullable: true),
                    Week11 = table.Column<bool>(type: "bit", nullable: true),
                    Week12 = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weeks", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_Weeks_Attendances_AttendanceId",
                        column: x => x.AttendanceId,
                        principalTable: "Attendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_CourseId",
                table: "Attendances",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId_CourseId",
                table: "Attendances",
                columns: new[] { "StudentId", "CourseId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weeks");

            migrationBuilder.DropTable(
                name: "Attendances");
        }
    }
}
