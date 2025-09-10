using System.ComponentModel.DataAnnotations;

namespace StudentEnrollment.Api.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
