using Lab_distributed_dbs.DAL;
using Lab_distributed_dbs.DAL.Settings;
using Lab_distributed_dbs.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Lab_distributed_dbs
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SqlController : ControllerBase
    {
        private readonly LabDbContext _context;
        public SqlController(LabDbContext context) => _context = context;

        [HttpPost]
        public IActionResult Fill()
        {
            if (_context.Students.Any() || _context.Courses.Any())
            {
                return BadRequest("Database already contains data.");
            }

            var student1 = new Student { FirstName = "Alice", LastName = "Johnson" };
            var student2 = new Student { FirstName = "Bob", LastName = "Smith" };
            var student3 = new Student { FirstName = "Charlie", LastName = "Brown" };

            var course1 = new Course { Title = "Algorithms", Credits = 3 };
            var course2 = new Course { Title = "Linear Algebra", Credits = 4 };
            var course3 = new Course { Title = "Databases", Credits = 3 };

            var studentCourse1 = new StudentCourse { Student = student1, Course = course1 };
            var studentCourse2 = new StudentCourse { Student = student2, Course = course1 };
            var studentCourse3 = new StudentCourse { Student = student2, Course = course2 };
            var studentCourse4 = new StudentCourse { Student = student3, Course = course3 };

            var enrollment1 = new StudentEnrollment
            {
                Student = student1,
                Course = course1,
                EnrollmentDate = DateTime.Now,
                Grade = "A"
            };
            var enrollment2 = new StudentEnrollment
            {
                Student = student2,
                Course = course2,
                EnrollmentDate = DateTime.Now,
                Grade = "B"
            };
            var enrollment3 = new StudentEnrollment
            {
                Student = student3,
                Course = course3,
                EnrollmentDate = DateTime.Now,
                Grade = "C"
            };

            _context.Students.AddRange(student1, student2, student3);
            _context.Courses.AddRange(course1, course2, course3);
            _context.StudentCourses.AddRange(studentCourse1, studentCourse2, studentCourse3, studentCourse4);
            _context.Set<StudentEnrollment>().AddRange(enrollment1, enrollment2, enrollment3);

            _context.SaveChanges();

            return Ok("Database filled with sample data.");
        }

        [HttpDelete]
        public IActionResult Clear()
        {
            _context.StudentCourses.RemoveRange(_context.StudentCourses);
            _context.Students.RemoveRange(_context.Students);
            _context.Courses.RemoveRange(_context.Courses);
            _context.Set<StudentEnrollment>().RemoveRange(_context.Set<StudentEnrollment>());

            _context.SaveChanges();

            return Ok("All data has been cleared from the database.");
        }
    }

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MongoController : ControllerBase
    {
        private readonly UpdateService _sync;
        private readonly StudentService _studentService;
        private readonly StudentEnrollmentService _enrollmentService;

        public MongoController(
            UpdateService sync,
            StudentService studentService,
            StudentEnrollmentService enrollmentService)
        {
            _sync = sync;
            _studentService = studentService;
            _enrollmentService = enrollmentService;
        }

        [HttpPut]
        public async Task<IActionResult> Update()
        {
            try
            {
                await _sync.Sync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return Ok("All data has been synchronized.");
        }

        [HttpDelete]
        public async Task<IActionResult> Clear(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient)
        {
            await mongoClient.DropDatabaseAsync(mongoDBSettings.Value.DatabaseName);
            return Ok("All data has been cleared.");
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents() => Ok(await _studentService.GetAsync());

        [HttpGet]
        public async Task<IActionResult> GetEnrollments() => Ok(await _enrollmentService.GetAsync());
    }
}