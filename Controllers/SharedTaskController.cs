using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserGuard_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedTaskController : ControllerBase
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IApplicationUserRepository applicationUserRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICourseRepository courseRepository;
        private readonly IMaintenanceTaskRepository _taskRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AcademyAPI _context;
        private readonly IMediator _mediator;

        public SharedTaskController(IEmployeeRepository employeeRepository,
            UserManager<ApplicationUser> userManager, AcademyAPI context,
            IApplicationUserRepository applicationUserRepository, RoleManager<IdentityRole> role
            , ICourseRepository courseRepository, IMaintenanceTaskRepository maintenanceTaskRepository, IMediator mediator)
        {
            _employeeRepository = employeeRepository;
            _userManager = userManager;
            this._context = context;
            this.applicationUserRepository = applicationUserRepository;
            this._roleManager = role;
            this.courseRepository = courseRepository;
            this._taskRepo = maintenanceTaskRepository;
            _mediator = mediator;
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


        [Authorize(Policy = "AdminOrSecretary")]
        [HttpPost("AddTask")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddTask([FromForm] MaintenanceTaskDto dto, [FromForm] List<IFormFile> imageFiles)
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

                var tecnecal = await _employeeRepository.GetEmployeeDetailsByIdAsync(dto.EmployeeId);

                if (employee.BranchId != tecnecal.BranchId)
                {
                    return Unauthorized(new GeneralResponse
                    {
                        Success = false,
                        Data = "This Employee is in another branch"
                    });
                }
                await _taskRepo.AddTaskAsync(dto, employee.BranchId, imageFiles);

                await _mediator.Publish(new UserRegisteredEvent(tecnecal.User.Email,
                    "You’ve Got a New Task – Please Review",
                    " Hello, \r\nA new maintenance task has been assigned to your account.\r\n" +
                    "Please log in to your dashboard to review the details and take any necessary action." +
                    "\r\n\r\nThank you for your dedication.\r\n\r\n– The Support Team\r\n\r\n"));

                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = "Task added successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = ex.Message
                });
            }
        }


        

        [Authorize(Policy = "AdminOrSecretaryOrMaintenance")]
        [HttpPatch("UpdateTask/{taskId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateTask(string taskId, [FromForm] MaintenanceTaskToUpdateDto dto, List<IFormFile>? newImages)
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


                await _taskRepo.UpdateTaskAsync(taskId, dto, employee.BranchId, newImages);
                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = "Task updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse
                {
                    Success = false,
                    Data = ex.Message
                });
            }
        }


    }
}
