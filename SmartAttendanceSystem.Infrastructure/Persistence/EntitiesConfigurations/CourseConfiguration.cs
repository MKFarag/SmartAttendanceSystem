
namespace SmartAttendanceSystem.Infrastructure.Persistence.EntitiesConfigurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasIndex(x => x.Name)
            .IsUnique();
        
        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .HasMaxLength(4);

        // Seed deduplicated courses data
        var courses = new List<Course>
        {
            // Computer Science Courses
            new() { Id = 1, Name = "Linear Algebra 1", Code = "r212", Level = 2 },
            new() { Id = 2, Name = "Numerical Analysis 1", Code = "r216", Level = 2 },
            new() { Id = 3, Name = "Introduction To Information Systems", Code = "r266", Level = 2 },
            new() { Id = 4, Name = "Object-Oriented Programming", Code = "r262", Level = 2 },
            new() { Id = 5, Name = "Computer Networks 1", Code = "r264", Level = 2 },
            new() { Id = 6, Name = "Probability Theory 1", Code = "r242", Level = 2 },
            new() { Id = 7, Name = "Advanced Operating Systems", Code = "r362", Level = 4 },
            new() { Id = 8, Name = "Web Programming Project", Code = "r366", Level = 4 },
            new() { Id = 9, Name = "Operations Research", Code = "r318", Level = 3 },
            new() { Id = 10, Name = "Computer Graphics", Code = "r364", Level = 4 },
            new() { Id = 11, Name = "Logic Programming", Code = "r372", Level = 4 },
            new() { Id = 12, Name = "Graph Theory", Code = "r368", Level = 4 },
            new() { Id = 13, Name = "Calculus 2", Code = "r211", Level = 2 },
            new() { Id = 14, Name = "Ordinary Differential Equations", Code = "r213", Level = 2 },
            new() { Id = 15, Name = "Database Systems", Code = "r263", Level = 2 },
            new() { Id = 16, Name = "Computer Architecture", Code = "r265", Level = 2 },
            new() { Id = 17, Name = "Structured Programming Basics", Code = "r261", Level = 2 },
            new() { Id = 18, Name = "Statistical Theory", Code = "r243", Level = 2 },
            new() { Id = 19, Name = "Abstract Algebra 1", Code = "r311", Level = 3 },
            new() { Id = 20, Name = "Ordinary Differential Equation Theory", Code = "r315", Level = 3 },
            new() { Id = 21, Name = "Operating Systems", Code = "r365", Level = 4 },
            new() { Id = 22, Name = "Computer Networks 2", Code = "r367", Level = 3 },
            new() { Id = 23, Name = "Algorithm Design And Analysis", Code = "r361", Level = 3 },
            new() { Id = 24, Name = "Web Programming", Code = "r363", Level = 4 },
            new() { Id = 25, Name = "Artificial Intelligence", Code = "r463", Level = 2 },
            new() { Id = 26, Name = "Advanced Computer Graphics", Code = "r471", Level = 4 },
            new() { Id = 27, Name = "Formal Languages", Code = "r461", Level = 2 },
            new() { Id = 28, Name = "Image Processing", Code = "r465", Level = 3 },
            new() { Id = 29, Name = "Signal Processing", Code = "r469", Level = 3 },
            new() { Id = 30, Name = "Parallel Processing", Code = "r467", Level = 4 },
            
            // Mathematics Courses
            new() { Id = 31, Name = "Dynamics 2", Code = "r233", Level = 2 },
            new() { Id = 32, Name = "Solid Geometry", Code = "r215", Level = 2 },
            new() { Id = 33, Name = "Statics 2", Code = "r231", Level = 2 },
            new() { Id = 34, Name = "Extended Analysis And General Relativity", Code = "r335", Level = 3 },
            new() { Id = 35, Name = "Real Analysis", Code = "r313", Level = 3 },
            new() { Id = 36, Name = "Electrostatics", Code = "r331", Level = 3 },
            new() { Id = 37, Name = "Analytical Mechanics", Code = "r333", Level = 3 },
            new() { Id = 38, Name = "Statistical Mechanics", Code = "r431", Level = 4 },
            new() { Id = 39, Name = "Fluid Theory 1", Code = "r433", Level = 4 },
            new() { Id = 40, Name = "Functional Analysis", Code = "413", Level = 4 },
            new() { Id = 41, Name = "Complex Analysis", Code = "r415", Level = 4 },
            new() { Id = 42, Name = "Partial Differential Equations 1", Code = "r414", Level = 4 },
            new() { Id = 43, Name = "Mathematical Analysis", Code = "r214", Level = 2 },
            new() { Id = 44, Name = "Vector Analysis", Code = "r234", Level = 2 },
            new() { Id = 45, Name = "Rigid Body Mechanics", Code = "r232", Level = 2 },
            new() { Id = 46, Name = "Special Functions", Code = "r218", Level = 2 },
            new() { Id = 47, Name = "Topology 1", Code = "r314", Level = 3 },
            new() { Id = 48, Name = "Differential Geometry", Code = "r316", Level = 3 },
            new() { Id = 49, Name = "Abstract Algebra 2", Code = "r312", Level = 3 },
            new() { Id = 50, Name = "Quantum Mechanics 1", Code = "r332", Level = 3 },
            new() { Id = 51, Name = "Elasticity Theory 1", Code = "r334", Level = 3 },
            // Using existing:
            //      Calculus 2 (Id: 13)
            //      Ordinary Differential Equations (Id: 14)
            //      Abstract Algebra 1 (Id: 19)
            //      Ordinary Differential Equation Theory (Id: 20)
            
            // Physics Courses
            new() { Id = 52, Name = "Oscillating Current And Waves", Code = "f215", Level = 2 },
            new() { Id = 53, Name = "Modern Physics", Code = "f213", Level = 2 },
            new() { Id = 54, Name = "Electromagnetism", Code = "f211", Level = 2 },
            new() { Id = 55, Name = "Crystal Physics", Code = "f327", Level = 3 },
            new() { Id = 56, Name = "Statistical Physics", Code = "f343", Level = 3 },
            new() { Id = 57, Name = "Biophysics", Code = "f365", Level = 3 },
            new() { Id = 58, Name = "Solid State Physics 1", Code = "f325", Level = 2 },
            new() { Id = 59, Name = "Nuclear Physics 1", Code = "f333", Level = 4 },
            new() { Id = 60, Name = "Atomic Physics 2", Code = "f371", Level = 3 },
            new() { Id = 61, Name = "Solid State Physics 2", Code = "f423", Level = 4 },
            new() { Id = 62, Name = "Nuclear Physics 2", Code = "f431", Level = 4 },
            new() { Id = 63, Name = "Electrodynamics", Code = "f441", Level = 4 },
            new() { Id = 64, Name = "Spectral Analysis", Code = "f477", Level = 3 },
            new() { Id = 65, Name = "Quantum Mechanics 2", Code = "f445", Level = 3 },
            new() { Id = 66, Name = "Molecular Physics", Code = "f479", Level = 4 },
            new() { Id = 67, Name = "Physical Optics", Code = "f212", Level = 4 },
            new() { Id = 68, Name = "Thermodynamics", Code = "f214", Level = 4 },
            new() { Id = 69, Name = "Atomic Physics 1", Code = "f272", Level = 4 },
            new() { Id = 70, Name = "Quantum Physics 1", Code = "f342", Level = 3 },
            new() { Id = 71, Name = "Plasma Physics", Code = "f384", Level = 3 },
            new() { Id = 72, Name = "Electronics 1", Code = "f324", Level = 3 },
            new() { Id = 73, Name = "Environmental Physics", Code = "f392", Level = 3 },
            // Using existing:
            //      Linear Algebra 1 (Id: 1)
            //      Numerical Analysis 1 (Id: 2)
            //      Calculus 2 (Id: 13)
            //      Ordinary Differential Equations (Id: 14)
            //      Solid Geometry (Id: 33)
            //      Mathematical Analysis (Id: 44)
            
            // Chemistry Courses
            new() { Id = 74, Name = "Organic Chemistry 3", Code = "k213", Level = 4 },
            new() { Id = 75, Name = "Organic Chemistry 2", Code = "k211", Level = 4 },
            new() { Id = 76, Name = "Volumetric And Gravimetric Analysis", Code = "k241", Level = 4 },
            new() { Id = 77, Name = "Kinetics And Acid Base", Code = "k231", Level = 4 },
            new() { Id = 78, Name = "Mathematical Statistics", Code = "241", Level = 4 },
            new() { Id = 79, Name = "Representative Elements Chemistry", Code = "k221", Level = 4 },
            new() { Id = 80, Name = "Physical Organic Chemistry", Code = "k311", Level = 3 },
            new() { Id = 81, Name = "Electrochemical And Chromatographic Methods", Code = "k341", Level = 3 },
            new() { Id = 82, Name = "Heterocyclic Rings And Dyes Chemistry", Code = "k313", Level = 3 },
            new() { Id = 83, Name = "Solid And Colloidal Chemistry", Code = "k331", Level = 3 },
            new() { Id = 84, Name = "Natural Products And Polymers Chemistry", Code = "k315", Level = 3 },
            new() { Id = 85, Name = "Coordination Chemistry And Electrochemistry", Code = "k321", Level = 3 },
            new() { Id = 86, Name = "Instrumental Analysis 1", Code = "k441", Level = 4 },
            new() { Id = 87, Name = "Environmental Organic Chemistry", Code = "k411", Level = 3 },
            new() { Id = 88, Name = "Petroleum And Petrochemicals Chemistry", Code = "k413", Level = 4 },
            new() { Id = 89, Name = "Electrochemistry And Corrosion", Code = "k431", Level = 4 },
            new() { Id = 90, Name = "Thermal Analysis", Code = "k433", Level = 3 },
            new() { Id = 91, Name = "Organic Chemistry 6", Code = "k214", Level = 2 },
            new() { Id = 92, Name = "Nuclear And Laser Chemistry", Code = "k234", Level = 2 },
            new() { Id = 93, Name = "Organic Chemistry 7", Code = "k216", Level = 2 },
            new() { Id = 94, Name = "Organic Chemistry 5", Code = "k212", Level = 2 },
            new() { Id = 95, Name = "Transition Elements Chemistry 1", Code = "k222", Level = 2 },
            new() { Id = 96, Name = "Thermodynamics 1", Code = "k232", Level = 2 },
            new() { Id = 97, Name = "Transition Elements Chemistry 2 And Cement", Code = "k324", Level = 3 },
            new() { Id = 98, Name = "Thermodynamics 2 And Spectrum", Code = "k334", Level = 3 },
            new() { Id = 99, Name = "Photochemistry And Cyclo Reactions", Code = "k316", Level = 3 }
        };

        builder.HasData(courses);
    }
}
