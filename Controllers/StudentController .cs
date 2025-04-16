using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserGuard_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "StudentPolicy")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(IStudentRepository studentRepository, UserManager<ApplicationUser> userManager)
        {
            _studentRepository = studentRepository;
            _userManager = userManager;
        }


        [HttpGet("MyCourses")]
        public async Task<IActionResult> GetMyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });

            var student = await _studentRepository.GetStudentByAppUserIdAsync(user.Id);
            if (student == null)
                return Unauthorized(new GeneralResponse { Success = false, Data = "You're not a student" });

            var courses = await _studentRepository.GetStudentCoursesWithGradesAsync(student.Id);

            return Ok(new GeneralResponse
            {
                Success = true,
                Data = courses
            });
        }


        [HttpGet("Mycolleagues")]
        public async Task<IActionResult> Getcolleagues()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });

            var student = await _studentRepository.GetStudentByAppUserIdAsync(user.Id);
            if (student == null)
                return Unauthorized(new GeneralResponse { Success = false, Data = "You're not a student" });

            var mycolleagues = await _studentRepository.GetStudentsInSameCourses(student.Id);

            if (mycolleagues == null || !mycolleagues.Any())
            {
                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = "No students found in your courses"
                });
            }

            return Ok(new GeneralResponse
            {
                Success = true,
                Data = mycolleagues
            });
        }

    }
}
