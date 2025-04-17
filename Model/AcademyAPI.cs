
using Microsoft.EntityFrameworkCore;

namespace UserGuard_API.Model
{
    public class AcademyAPI:IdentityDbContext<ApplicationUser>
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<MaintenanceTask> MaintenanceTasks { get; set; }
        public DbSet<Employee> Employees {get; set;}

        public DbSet<Student> Students { get; set; }

        public DbSet<Grade> Grades { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        public DbSet<MaintenanceTaskImage> MaintenanceTaskImages { get; set; }


        public AcademyAPI(DbContextOptions<AcademyAPI> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // إضافة العلاقة بين ApplicationUser و Employee
            builder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.ApplicationUserId);


            builder.Entity<Branch>()
             .HasOne(b => b.Manager)
             .WithMany()
             .HasForeignKey(b => b.ManagerId)
             .OnDelete(DeleteBehavior.SetNull);
        }

    }
}
