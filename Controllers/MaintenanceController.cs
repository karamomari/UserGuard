
namespace UserGuard_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceTaskRepository _taskRepo;
        private readonly IEmployeeRepository employeeRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public MaintenanceController(IMaintenanceTaskRepository taskRepo, UserManager<ApplicationUser> userManager,
           IEmployeeRepository employeeRepository)
        {
            _taskRepo = taskRepo;
            _userManager = userManager;
            this.employeeRepository = employeeRepository;

        }

        [HttpGet("MyTasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employee = await employeeRepository.GetEmployeeByUserIdAsync(user.Id);
            if (employee == null)
            {
                return Unauthorized(new GeneralResponse
                {
                    Success=false,
                    Data = "You're not an employee"
                });
            }
            var tasks = await _taskRepo.GetTasksByEmployeeIdAsyncDto(employee.Id);

            return Ok(new GeneralResponse
            {
                Success = true,
                Data = tasks
            });
        }
    }
}
