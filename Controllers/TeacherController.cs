
using Microsoft.EntityFrameworkCore;

namespace UserGuard_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "TeacherPolicy")]
    public class TeacherController : ControllerBase
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AcademyAPI context;

        public TeacherController(IGradeRepository gradeRepository, IScheduleRepository scheduleRepository,
            UserManager<ApplicationUser> userManager, ICourseRepository courseRepository,
            IEmployeeRepository employeeRepository, AcademyAPI context)
        {
            _gradeRepository = gradeRepository;
            _scheduleRepository = scheduleRepository;
            _userManager = userManager;
            _courseRepository = courseRepository;
            _employeeRepository = employeeRepository;
            this.context = context;
        }







        [HttpGet("MyCourses")]
        public async Task<IActionResult> GetMyCourses()
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });

                //var teacherEmployee = context.Employees.Where(e => e.ApplicationUserId == user.Id).FirstOrDefault();


                var teacherEmployee = await _employeeRepository.GetEmployeeByUserIdAsync(user.Id);

                if (teacherEmployee == null)
                    return StatusCode(403, new GeneralResponse { Success = false, Data = "You're not authorized to view teacher courses." });


                var courses = await _courseRepository.GetCoursesByTeacherIdAsync(teacherEmployee.Id);
                var result = courses.Select(c => new CourseToReturnDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreditHours = c.CreditHours
                });

                return Ok(new GeneralResponse { Success = true, Data = result });
            }

            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = ex.Message });
            }
        }




        [HttpPost("AddGrade")]
        public async Task<IActionResult> AddGrade([FromBody] GradeDto gradeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new GeneralResponse
                    {
                        Success = false,
                        Data = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });

                var grade = await _gradeRepository.AddGradeAsync(gradeDto, user.Id);
                if (grade == null)
                    return BadRequest(new GeneralResponse { Success = false, Data = "Failed to add grade" });

                return Ok(new GeneralResponse { Success = true, Data = grade });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = ex.Message });
            }
        }



        [HttpPatch("UpdateGrade")]
        public async Task<IActionResult> UpdateGrade([FromBody] GradeDto newgrade)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(new GeneralResponse
                    {
                        Success = false,
                        Data = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });

                var Teacher = await _employeeRepository.GetEmployeeByUserIdAsync(user.Id);
                if (Teacher == null)
                {
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized You are not Teacher" });
                }
                var result = await _gradeRepository.UpdateGradeAsync(newgrade, Teacher.Id);
                if (result == null)
                {
                    return BadRequest(new GeneralResponse { Success = false, Data = "Invalid course or student, or you're not authorized to modify the grade." });
                }
                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = ex.Message });
            }
        }


        [HttpPost("ScheduleExam")]
        public async Task<IActionResult> ScheduleExam([FromBody] ScheduleDto scheduleDto)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(new GeneralResponse
                    {
                        Success = false,
                        Data = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });

                var schedule = await _scheduleRepository.AddExamScheduleAsync(scheduleDto, user.Id);
                if (schedule == null)
                    return BadRequest(new GeneralResponse { Success = false, Data = "Failed to schedule exam" });

                return Ok(new GeneralResponse { Success = true, Data = schedule });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = ex.Message });
            }
        }
    }
}
