using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StudentEnrollment.Api.Models;

namespace StudentEnrollment.Api.Data
{

    public class StudentEnrollmentDbContext : IdentityDbContext<IdentityUser>
    {
        public StudentEnrollmentDbContext(DbContextOptions<StudentEnrollmentDbContext> options)
      : base(options) { }

        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    }
}
