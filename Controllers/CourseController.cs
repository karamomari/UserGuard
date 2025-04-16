

namespace UserGuard_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {

        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmployeeRepository employeeRepository;

        public CourseController(ICourseRepository courseRepository, UserManager<ApplicationUser> userManager, IEmployeeRepository employeeRepository
            )
        {
            _courseRepository = courseRepository;
            _userManager = userManager;
            this.employeeRepository = employeeRepository;
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateCourse(CourseDto courseDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new GeneralResponse
                    {
                        Success = false,
                        Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var adminEmployee = await employeeRepository.GetEmployeeByUserIdAsync(currentUserId);
                if (adminEmployee == null)
                {
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });
                }


                if (!string.IsNullOrEmpty(courseDto.TeacherId))
                {
                    var teacher = await employeeRepository.GetEmployeeByEmployeeIdAsync(courseDto.TeacherId);

                    if (teacher == null || teacher.BranchId != adminEmployee.BranchId)
                    {
                        return BadRequest(new GeneralResponse
                        {
                            Success = false,
                            Data = "The selected teacher does not belong to your branch."
                        });
                    }
                }

                var createdCourse = await _courseRepository.CreateCourseAsync(courseDto, adminEmployee.BranchId);

                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = createdCourse
                });
            }
            catch (Exception e)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = e.Message });
            }
        }



        [HttpGet("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetCourse(string id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var adminEmployee = await employeeRepository.GetEmployeeByUserIdAsync(currentUserId);
                if (adminEmployee == null)
                {
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });
                }
                var course = await _courseRepository.GetCourseByIdAsync(id);
                if (course == null)
                {
                    return NotFound(new GeneralResponse { Success = false, Data = "Course not found." });
                }
                if (course.BranchId != adminEmployee.BranchId)
                {
                    //return Forbid();
                    return StatusCode(403, new GeneralResponse { Success = false, Data = "Access denied. This course belongs to another branch." });
                }
                return Ok(new GeneralResponse { Success = true, Data = course });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = ex.Message });
            }
        }


        [HttpGet("GetAllBranch")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllBranch()
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var adminEmployee = await employeeRepository.GetEmployeeByUserIdAsync(currentUserId);
                if (adminEmployee == null)
                {
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });
                }
                var Course = await _courseRepository.GetCoursesForAdminAsync(adminEmployee.Id);
                if (Course == null)
                {
                    return StatusCode(204, new GeneralResponse { Data = "Success but in your Branch dosn't has any Courses,Success=true" });
                }
                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = Course
                });
            }
            catch(Exception e)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = e.Message
                });
            }
        }


        [HttpPatch("updatecourse/{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateCourse([FromBody] CourseToUpdateDto courseToUpdate, string id)
        {
            
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new GeneralResponse
                    {
                        Success = false,
                        Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var adminEmployee = await employeeRepository.GetEmployeeByUserIdAsync(currentUserId);
                if (adminEmployee == null)
                {
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });
                }

                var course = await _courseRepository.GetCourseByIdAsync(id);
                if (course == null)
                {
                    return NotFound(new GeneralResponse { Success = false, Data = "This course doesn't exist." });
                }

                if (course.BranchId != adminEmployee.BranchId)
                {
                    return StatusCode(403, new GeneralResponse
                    {
                        Success = false,
                        Data = "You are not authorized to update a course from another branch."
                    });
                }

                if (!string.IsNullOrEmpty(courseToUpdate.TeacherId))
                {
                    var teacher = await employeeRepository.GetEmployeeByEmployeeIdAsync(courseToUpdate.TeacherId);
                    if (teacher == null)
                    {
                        return BadRequest(new GeneralResponse { Success = false, Data = "This teacher doesn't exist." });
                    }

                    if (teacher.BranchId != adminEmployee.BranchId)
                    {
                        return StatusCode(403, new GeneralResponse
                        {
                            Success = false,
                            Data = "You are not authorized to assign a teacher from another branch."
                        });
                    }
                }

                var updatedCourse = await _courseRepository.UpdateCourseAsync(courseToUpdate, id);

                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = updatedCourse
                });
            }
            catch (Exception e)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = e.Message
                });
            }
        }



        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteCourse(string id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var adminEmployee = await employeeRepository.GetEmployeeByUserIdAsync(currentUserId);
                if (adminEmployee == null)
                {
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Unauthorized" });
                }

                var course = await _courseRepository.GetCourseByIdAsync(id);
                if (course == null)
                {
                    return NotFound(new GeneralResponse { Success = false, Data = "Course not found." });
                }

                if (course.BranchId != adminEmployee.BranchId)
                {
                    return StatusCode(403, new GeneralResponse { Success = false, Data = "You are not authorized to delete this course." });
                }

                await _courseRepository.DeleteCourseAsync(course);
                return Ok(new GeneralResponse { Success = true, Data = "Course deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = ex.Message });
            }
        }


        //updateee end point
        [HttpGet("GetAllBranchToSuperAdmin")]
        public async Task<IActionResult> GetAllCourse()
        {
           var courses= await _courseRepository.GetAllAsync();
            return Ok(new GeneralResponse
            {
                Success=true,
                Data=courses
            });
        }



        [HttpGet]
        //للطلاب او بدريش بمين 
        public async Task<IActionResult> GetAllStudentCourses()
        {
            var studentCourses = await _courseRepository.GetAllStudentCoursesAsync();

            return Ok(new GeneralResponse
            {
                Success = true,
                Data = studentCourses
            });
        }

    }
}
