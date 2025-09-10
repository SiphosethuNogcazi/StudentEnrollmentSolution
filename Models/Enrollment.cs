namespace StudentEnrollment.Api.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public int CourseId { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public Course? Course { get; set; }
    }
}
