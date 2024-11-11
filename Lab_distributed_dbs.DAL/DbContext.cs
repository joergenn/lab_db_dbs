using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Lab_distributed_dbs.DAL
{
    public class LabDbContext : DbContext
    {
        public LabDbContext(DbContextOptions<LabDbContext> options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<StudentEnrollment> StudentEnrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);

            modelBuilder.Entity<StudentEnrollment>()
                .HasKey(se => se.EnrollmentId);

            modelBuilder.Entity<StudentEnrollment>()
                .HasOne(se => se.Student)
                .WithMany(s => s.StudentEnrollments)
                .HasForeignKey(se => se.StudentId);

            modelBuilder.Entity<StudentEnrollment>()
                .HasOne(se => se.Course)
                .WithMany()
                .HasForeignKey(se => se.CourseId);
        }

    }
}
