
using StudentEnrollment.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Api.Data;
using System.Security.Claims;

namespace StudentEnrollmentSolution.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // only logged-in users can access
    public class CoursesController : ControllerBase
    {
        private readonly StudentEnrollmentDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CoursesController(StudentEnrollmentDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/courses
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(courses);
        }

        // POST: api/courses
        [HttpPost]
        [Authorize(Roles = "Admin")] // only Admin can create courses
        public async Task<IActionResult> CreateCourse([FromBody] Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCourses), new { id = course.Id }, course);
        }

        // POST: api/courses/enroll/5
        [HttpPost("enroll/{courseId}")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId))
                return BadRequest(new { message = "Already enrolled in this course" });

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrolled successfully" });
        }

        // GET: api/courses/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myCourses = await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .Select(e => new { e.Course.Id, e.Course.Title })
                .ToListAsync();

            return Ok(myCourses);
        }

        // DELETE: api/courses/deregister/5
        [HttpDelete("deregister/{courseId}")]
        public async Task<IActionResult> Deregister(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment == null)
                return NotFound(new { message = "Not enrolled in this course" });

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deregistered successfully" });
        }
    }
}
