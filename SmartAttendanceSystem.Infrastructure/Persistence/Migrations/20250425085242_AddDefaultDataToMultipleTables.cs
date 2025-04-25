using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartAttendanceSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultDataToMultipleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Code", "Level", "Name" },
                values: new object[,]
                {
                    { 1, "r212", 2, "Linear Algebra 1" },
                    { 2, "r216", 2, "Numerical Analysis 1" },
                    { 3, "r266", 2, "Introduction To Information Systems" },
                    { 4, "r262", 2, "Object-Oriented Programming" },
                    { 5, "r264", 2, "Computer Networks 1" },
                    { 6, "r242", 2, "Probability Theory 1" },
                    { 7, "r362", 4, "Advanced Operating Systems" },
                    { 8, "r366", 4, "Web Programming Project" },
                    { 9, "r318", 3, "Operations Research" },
                    { 10, "r364", 4, "Computer Graphics" },
                    { 11, "r372", 4, "Logic Programming" },
                    { 12, "r368", 4, "Graph Theory" },
                    { 13, "r211", 2, "Calculus 2" },
                    { 14, "r213", 2, "Ordinary Differential Equations" },
                    { 15, "r263", 2, "Database Systems" },
                    { 16, "r265", 2, "Computer Architecture" },
                    { 17, "r261", 2, "Structured Programming Basics" },
                    { 18, "r243", 2, "Statistical Theory" },
                    { 19, "r311", 3, "Abstract Algebra 1" },
                    { 20, "r315", 3, "Ordinary Differential Equation Theory" },
                    { 21, "r365", 4, "Operating Systems" },
                    { 22, "r367", 3, "Computer Networks 2" },
                    { 23, "r361", 3, "Algorithm Design And Analysis" },
                    { 24, "r363", 4, "Web Programming" },
                    { 25, "r463", 2, "Artificial Intelligence" },
                    { 26, "r471", 4, "Advanced Computer Graphics" },
                    { 27, "r461", 2, "Formal Languages" },
                    { 28, "r465", 3, "Image Processing" },
                    { 29, "r469", 3, "Signal Processing" },
                    { 30, "r467", 4, "Parallel Processing" },
                    { 31, "r233", 2, "Dynamics 2" },
                    { 32, "r215", 2, "Solid Geometry" },
                    { 33, "r231", 2, "Statics 2" },
                    { 34, "r335", 3, "Extended Analysis And General Relativity" },
                    { 35, "r313", 3, "Real Analysis" },
                    { 36, "r331", 3, "Electrostatics" },
                    { 37, "r333", 3, "Analytical Mechanics" },
                    { 38, "r431", 4, "Statistical Mechanics" },
                    { 39, "r433", 4, "Fluid Theory 1" },
                    { 40, "413", 4, "Functional Analysis" },
                    { 41, "r415", 4, "Complex Analysis" },
                    { 42, "r414", 4, "Partial Differential Equations 1" },
                    { 43, "r214", 2, "Mathematical Analysis" },
                    { 44, "r234", 2, "Vector Analysis" },
                    { 45, "r232", 2, "Rigid Body Mechanics" },
                    { 46, "r218", 2, "Special Functions" },
                    { 47, "r314", 3, "Topology 1" },
                    { 48, "r316", 3, "Differential Geometry" },
                    { 49, "r312", 3, "Abstract Algebra 2" },
                    { 50, "r332", 3, "Quantum Mechanics 1" },
                    { 51, "r334", 3, "Elasticity Theory 1" },
                    { 52, "f215", 2, "Oscillating Current And Waves" },
                    { 53, "f213", 2, "Modern Physics" },
                    { 54, "f211", 2, "Electromagnetism" },
                    { 55, "f327", 3, "Crystal Physics" },
                    { 56, "f343", 3, "Statistical Physics" },
                    { 57, "f365", 3, "Biophysics" },
                    { 58, "f325", 2, "Solid State Physics 1" },
                    { 59, "f333", 4, "Nuclear Physics 1" },
                    { 60, "f371", 3, "Atomic Physics 2" },
                    { 61, "f423", 4, "Solid State Physics 2" },
                    { 62, "f431", 4, "Nuclear Physics 2" },
                    { 63, "f441", 4, "Electrodynamics" },
                    { 64, "f477", 3, "Spectral Analysis" },
                    { 65, "f445", 3, "Quantum Mechanics 2" },
                    { 66, "f479", 4, "Molecular Physics" },
                    { 67, "f212", 4, "Physical Optics" },
                    { 68, "f214", 4, "Thermodynamics" },
                    { 69, "f272", 4, "Atomic Physics 1" },
                    { 70, "f342", 3, "Quantum Physics 1" },
                    { 71, "f384", 3, "Plasma Physics" },
                    { 72, "f324", 3, "Electronics 1" },
                    { 73, "f392", 3, "Environmental Physics" },
                    { 74, "k213", 4, "Organic Chemistry 3" },
                    { 75, "k211", 4, "Organic Chemistry 2" },
                    { 76, "k241", 4, "Volumetric And Gravimetric Analysis" },
                    { 77, "k231", 4, "Kinetics And Acid Base" },
                    { 78, "241", 4, "Mathematical Statistics" },
                    { 79, "k221", 4, "Representative Elements Chemistry" },
                    { 80, "k311", 3, "Physical Organic Chemistry" },
                    { 81, "k341", 3, "Electrochemical And Chromatographic Methods" },
                    { 82, "k313", 3, "Heterocyclic Rings And Dyes Chemistry" },
                    { 83, "k331", 3, "Solid And Colloidal Chemistry" },
                    { 84, "k315", 3, "Natural Products And Polymers Chemistry" },
                    { 85, "k321", 3, "Coordination Chemistry And Electrochemistry" },
                    { 86, "k441", 4, "Instrumental Analysis 1" },
                    { 87, "k411", 3, "Environmental Organic Chemistry" },
                    { 88, "k413", 4, "Petroleum And Petrochemicals Chemistry" },
                    { 89, "k431", 4, "Electrochemistry And Corrosion" },
                    { 90, "k433", 3, "Thermal Analysis" },
                    { 91, "k214", 2, "Organic Chemistry 6" },
                    { 92, "k234", 2, "Nuclear And Laser Chemistry" },
                    { 93, "k216", 2, "Organic Chemistry 7" },
                    { 94, "k212", 2, "Organic Chemistry 5" },
                    { 95, "k222", 2, "Transition Elements Chemistry 1" },
                    { 96, "k232", 2, "Thermodynamics 1" },
                    { 97, "k324", 3, "Transition Elements Chemistry 2 And Cement" },
                    { 98, "k334", 3, "Thermodynamics 2 And Spectrum" },
                    { 99, "k316", 3, "Photochemistry And Cyclo Reactions" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Computer Science" },
                    { 2, "Mathematics" },
                    { 3, "Physics" },
                    { 4, "Chemistry" }
                });

            migrationBuilder.InsertData(
                table: "DepartmentCourses",
                columns: new[] { "CourseId", "DepartmentId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 11, 1 },
                    { 12, 1 },
                    { 13, 1 },
                    { 14, 1 },
                    { 15, 1 },
                    { 16, 1 },
                    { 17, 1 },
                    { 18, 1 },
                    { 19, 1 },
                    { 20, 1 },
                    { 21, 1 },
                    { 22, 1 },
                    { 23, 1 },
                    { 24, 1 },
                    { 25, 1 },
                    { 26, 1 },
                    { 27, 1 },
                    { 28, 1 },
                    { 29, 1 },
                    { 30, 1 },
                    { 13, 2 },
                    { 14, 2 },
                    { 19, 2 },
                    { 20, 2 },
                    { 31, 2 },
                    { 32, 2 },
                    { 33, 2 },
                    { 34, 2 },
                    { 35, 2 },
                    { 36, 2 },
                    { 37, 2 },
                    { 38, 2 },
                    { 39, 2 },
                    { 40, 2 },
                    { 41, 2 },
                    { 42, 2 },
                    { 43, 2 },
                    { 44, 2 },
                    { 45, 2 },
                    { 46, 2 },
                    { 47, 2 },
                    { 48, 2 },
                    { 49, 2 },
                    { 50, 2 },
                    { 51, 2 },
                    { 1, 3 },
                    { 2, 3 },
                    { 13, 3 },
                    { 14, 3 },
                    { 33, 3 },
                    { 44, 3 },
                    { 52, 3 },
                    { 53, 3 },
                    { 54, 3 },
                    { 55, 3 },
                    { 56, 3 },
                    { 57, 3 },
                    { 58, 3 },
                    { 59, 3 },
                    { 60, 3 },
                    { 61, 3 },
                    { 62, 3 },
                    { 63, 3 },
                    { 64, 3 },
                    { 65, 3 },
                    { 66, 3 },
                    { 67, 3 },
                    { 68, 3 },
                    { 69, 3 },
                    { 70, 3 },
                    { 71, 3 },
                    { 72, 3 },
                    { 73, 3 },
                    { 74, 4 },
                    { 75, 4 },
                    { 76, 4 },
                    { 77, 4 },
                    { 78, 4 },
                    { 79, 4 },
                    { 80, 4 },
                    { 81, 4 },
                    { 82, 4 },
                    { 83, 4 },
                    { 84, 4 },
                    { 85, 4 },
                    { 86, 4 },
                    { 87, 4 },
                    { 88, 4 },
                    { 89, 4 },
                    { 90, 4 },
                    { 91, 4 },
                    { 92, 4 },
                    { 93, 4 },
                    { 94, 4 },
                    { 95, 4 },
                    { 96, 4 },
                    { 97, 4 },
                    { 98, 4 },
                    { 99, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 7, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 8, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 9, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 10, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 11, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 12, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 13, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 14, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 15, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 16, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 17, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 18, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 19, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 20, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 21, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 22, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 23, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 24, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 25, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 26, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 27, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 28, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 29, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 30, 1 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 13, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 14, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 19, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 20, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 31, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 32, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 33, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 34, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 35, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 36, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 37, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 38, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 39, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 40, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 41, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 42, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 43, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 44, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 45, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 46, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 47, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 48, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 49, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 50, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 51, 2 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 13, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 14, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 33, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 44, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 52, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 53, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 54, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 55, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 56, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 57, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 58, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 59, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 60, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 61, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 62, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 63, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 64, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 65, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 66, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 67, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 68, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 69, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 70, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 71, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 72, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 73, 3 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 74, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 75, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 76, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 77, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 78, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 79, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 80, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 81, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 82, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 83, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 84, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 85, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 86, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 87, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 88, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 89, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 90, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 91, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 92, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 93, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 94, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 95, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 96, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 97, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 98, 4 });

            migrationBuilder.DeleteData(
                table: "DepartmentCourses",
                keyColumns: new[] { "CourseId", "DepartmentId" },
                keyValues: new object[] { 99, 4 });

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
