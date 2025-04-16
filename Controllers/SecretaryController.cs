using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UserGuard_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SecretaryPolicy")]
    public class SecretaryController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMaintenanceTaskRepository _taskRepo;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMediator _mediator;



        public SecretaryController(IStudentRepository studentRepository, IEmployeeRepository employeeRepository,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,ICourseRepository courseRepository,IMaintenanceTaskRepository maintenanceTaskRepository
            ,IMediator _mediator)
        {
            _studentRepository = studentRepository;
            _employeeRepository = employeeRepository;
            _courseRepository = courseRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._taskRepo = maintenanceTaskRepository;
            this._mediator = _mediator;
        }



        private async Task<(bool Success, IActionResult? ErrorResult, Employee? Employee)> TryGetUserIdAndEmployee()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return (false, Unauthorized(new GeneralResponse { Success = false, Data = "User not found" }), null);

            var employee = await _employeeRepository.GetEmployeeByUserIdAsync(userId);
            if (employee == null)
                return (false, Unauthorized(new GeneralResponse { Success = false, Data = "You're not an Employee" }), null);

            return (true, null, employee);
        }


        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var (success, errorResult, employee) = await TryGetUserIdAndEmployee();
                if (!success)
                    return errorResult!;

                var courses = await _courseRepository.GetCoursesByBranchId(employee.BranchId);
                if (courses == null || courses.Count == 0)
                {
                    return Ok(new GeneralResponse { Success = true, Data = "No courses found for your branch." });
                }
                var courseDtos = courses.Select(c => new CourseToReturnDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreditHours = c.CreditHours
                }).ToList();

                return Ok(new GeneralResponse { Success = true, Data = courseDtos });
            }
            catch (Exception e)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = e.Message });
            }
        }



        [HttpPost("RegisterStudent")]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentToCreateDto studentDto)
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

                var (success, errorResult, employee) = await TryGetUserIdAndEmployee();
                if (!success)
                    return errorResult!;

                var existingUser = await userManager.FindByEmailAsync(studentDto.Email);
                if (existingUser != null)
                {
                    return BadRequest(new GeneralResponse { Success = false, Data = "This email is already in use." });
                }

                var user = new ApplicationUser
                {
                    UserName = studentDto.UserName,
                    Email = studentDto.Email,
                    PhoneNumber = studentDto.Phone,
                    FirstName = studentDto.FirstName,
                    LastName = studentDto.LastName,
                    EmailConfirmed = true,
                    IsActive = true,
                    AccountCreatedAt = DateTime.UtcNow,
                    LockoutEnabled = false,
                };

                var resultCreate = await userManager.CreateAsync(user, studentDto.Password);
                if (!resultCreate.Succeeded)
                {
                    var errors = string.Join(", ", resultCreate.Errors.Select(e => e.Description));
                    return BadRequest(new GeneralResponse { Success = false, Data = errors });
                }

                // التحقق من وجود الدور Student وإنشاؤه إذا لزم
                if (!await roleManager.RoleExistsAsync("Student"))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole("Student"));
                    if (!roleResult.Succeeded)
                    {
                        return BadRequest(new GeneralResponse { Success = false, Data = "Failed to create role Student" });
                    }
                }

                // إسناد الدور للمستخدم
                var roleAssignResult = await userManager.AddToRoleAsync(user, "Student");
                if (!roleAssignResult.Succeeded)
                {
                    var errors = string.Join(", ", roleAssignResult.Errors.Select(e => e.Description));
                    return BadRequest(new GeneralResponse { Success = false, Data = $"Failed to assign role Student: {errors}" });
                }

                // إضافة الطالب لقاعدة البيانات
                var result = await _studentRepository.Add(studentDto, employee.BranchId, user.Id);
                if (result == null)
                {
                    return BadRequest(new GeneralResponse { Success = false, Data = "Failed to add student" });
                }

                // إرسال رد ناجح

                await _mediator.Publish(new UserRegisteredEvent(user.Email, "Congratulations", "you have registered. You can log in."));


                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = new
                    {
                        StudentId = result.Id,
                        FullName = result.FullName,
                        Courses = result.StudentCourses.Select(sc => sc.CourseId).ToList()
                    }
                });
            }
            catch (Exception e)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = e.Message });
            }
        }



        [HttpGet("Students")]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                var (success, errorResult, employee) = await TryGetUserIdAndEmployee();
                if (!success)
                    return errorResult!;

                var students = await _studentRepository.GetAllStudentsByBranchId(employee.BranchId);
          
                return Ok(new GeneralResponse { Success = true, Data = students });
            }
            catch (Exception e) { return BadRequest(new GeneralResponse { Success = false, Data = e.Message }); }

        }



        [HttpPatch("UpdateStudent/{studentId}")]
        public async Task<IActionResult> UpdateStudent([FromBody] StudentToUpdateDto studentDto,string studentId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new GeneralResponse {Success=false,
                        Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)});
                }
                var (success, errorResult, employee) = await TryGetUserIdAndEmployee();
                if (!success)
                    return errorResult!;

                var student = await _studentRepository.GetByIdIfInSameBranch(studentId, employee.BranchId);
                if (student == null)
                    return Unauthorized(new GeneralResponse { Success = false, Data = "This student is in another branch or not found" });

                var updatedStudent = await _studentRepository.Update(studentDto, studentId);
                if (updatedStudent == null)
                    return NotFound(new GeneralResponse { Success = false, Data = "Student not found or unauthorized" });
                var StudentToReturen = new StudentToReturn
                {
                    Id = updatedStudent.Id,
                    FullName = updatedStudent.FullName,
                    BranchName = updatedStudent.Branch.Name,
                    Courses = updatedStudent.StudentCourses?
                  .Where(sc => sc.Course != null)
                  .Select(sc => sc.Course.Name)
                  .ToList() ?? new List<string>()
                };

                await _mediator.Publish(new UserRegisteredEvent(student.ApplicationUser.Email, "Heads Up", " Your Account Info Was Updated"));
               // await _mediator.Publish(new UserRegisteredEvent("karamomari20010@gmail.com", "sda", "sda"));


                return Ok(new GeneralResponse { Success = true, Data = StudentToReturen });
            }
            catch (Exception e)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = e.Message });
            }
        }


        [HttpDelete("DeleteStudent/{studentId}")]
        public async Task<IActionResult> DeleteStudent(string studentId)
        {
            try
            {
                var (success, errorResult, employee) = await TryGetUserIdAndEmployee();
                if (!success)
                    return errorResult!;

                var student = await _studentRepository.GetByIdIfInSameBranch(studentId, employee.BranchId);
                if (student == null)
                    return Unauthorized(new GeneralResponse { Success = false, Data = "This student is in another branch or not found" });

                var deleted = await _studentRepository.Delete(studentId);
                if (deleted == null)
                    return NotFound(new GeneralResponse { Success = false, Data = "Student not found or unauthorized" });



                return Ok(new GeneralResponse { Success = true, Data = "Student deleted successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = e.Message });
            }
        }




        [HttpGet("Student/{studentId}")]
        public async Task<IActionResult> GetStudentById(string studentId)
        {
            try
            {
                var (success, errorResult, employee) = await TryGetUserIdAndEmployee();
                if (!success)
                    return errorResult!;


                var student = await _studentRepository.GetByIdWithDetails(studentId);
                if (student == null)
                    return NotFound(new GeneralResponse { Success = false, Data = "Student not found" });

                if (student.BranchId != employee.BranchId)
                    return Unauthorized(new GeneralResponse { Success = false, Data = "This student is in another branch" });

                var StudentToReturen = new StudentToReturn
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    BranchName = student.Branch.Name,
                    Courses = student.StudentCourses?
                     .Where(sc => sc.Course != null)
                     .Select(sc => sc.Course.Name)
                     .ToList() ?? new List<string>()
                };
                return Ok(new GeneralResponse { Success = true, Data = StudentToReturen });
            }
            catch (Exception e)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = e.Message });
            }
        }


        [HttpPost("RegisterStudentInCourse")]
        public async Task<IActionResult> RegisterStudentInCourse([FromBody] RegisterCourseDto dto)
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
                var (success, errorResult, employee) = await TryGetUserIdAndEmployee();
                if (!success)
                    return errorResult!;

                var student = await _studentRepository.GetByIdIfInSameBranch(dto.StudentId, employee.BranchId);
                if (student == null)
                    return Unauthorized(new GeneralResponse { Success = false, Data = "Student not found or not in your branch" });

                var result = await _studentRepository.RegisterStudentInCourse(dto.StudentId, dto.CourseName);
                if (!result)
                    return BadRequest(new GeneralResponse { Success = false, Data = "Registration failed. Course might not exist or already registered." });

                return Ok(new GeneralResponse { Success = true, Data = "Student registered successfully in course." });
            }
            catch (Exception e)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = e.Message });
            }
        }



 
    }
}
