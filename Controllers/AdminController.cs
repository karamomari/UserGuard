


namespace UserGuard_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IApplicationUserRepository applicationUserRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICourseRepository courseRepository;
        private readonly IMaintenanceTaskRepository _taskRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AcademyAPI _context;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;


        public AdminController(IEmployeeRepository employeeRepository,
            UserManager<ApplicationUser> userManager, AcademyAPI context, 
            IApplicationUserRepository applicationUserRepository,RoleManager<IdentityRole> role
            , ICourseRepository courseRepository,IMaintenanceTaskRepository maintenanceTaskRepository
            , IMediator mediator, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _userManager = userManager;
            this._context = context;
            this.applicationUserRepository = applicationUserRepository;
             this._roleManager=role;
            this.courseRepository = courseRepository;
            this._taskRepo = maintenanceTaskRepository;
            this._mediator = mediator;
            _mapper = mapper;

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


        [HttpGet("branch-employees")]
        public async Task<IActionResult> GetEmployeesInSameBranch()
        {
            //ما اجيب كل الموظفين مع بعض Pagination 
            try
            {

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                if (currentUserId == null)
                {
                    return Unauthorized(new GeneralResponse
                    {
                        Success = false,
                        Data = "You must be logged in to access this resource."
                    });

                }

                var employee = await _employeeRepository.GetEmployeeByUserIdAsync(currentUserId);

                if (employee == null)
                {

                    return NotFound(
                        new GeneralResponse
                        {
                            Success = false,
                            Data = "Employee associated with the user not found."
                        });
                }

                var employeesInSameBranch = await _employeeRepository.GetEmployeesByBranchIdAsync(employee.BranchId);

                if (employeesInSameBranch == null || !employeesInSameBranch.Any())
                {
                    return NotFound(new GeneralResponse { Success = false, Data = "No employees found in the same branch." });
                }

                return Ok(new GeneralResponse { Success = true, Data = employeesInSameBranch });
            }

            catch (Exception e)
            {
                return BadRequest(new GeneralResponse { Success=false,Data=e.Message});
            }
            
        }



        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDto dto)
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

                var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
                if (!roleExists)
                {
                    var role = new IdentityRole(dto.Role);
                    await _roleManager.CreateAsync(role);
                }

                var user = applicationUserRepository.CreateApplicationUser(dto);

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    return BadRequest(new GeneralResponse
                    {
                        Success = false,
                        Data = result.Errors.FirstOrDefault()?.Description ?? "An error occurred while creating the user."
                        //Data = result.Errors
                    });

                await _userManager.AddToRoleAsync(user, dto.Role);

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = await _context.Users
                    .Include(u => u.Employee)
                    .FirstOrDefaultAsync(u => u.Id == currentUserId);

                if (currentUser?.Employee == null)
                    return Forbid();
                // ما نرجّع GeneralResponse هون لأن Forbid بيرجع 403 تلقائيًا
                await _employeeRepository.AddEmployee(dto, user.Id, currentUser.Employee.BranchId);

                await _mediator.Publish(new UserRegisteredEvent(dto.Email, "Congratulations", "you have registered. You can log in."));


                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = "Employee registered successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse
                {
                    Success = false,
                    Data = $"An error occurred: {ex.Message}"
                });
            }
        }



        [HttpGet("employee/{id}")]
        public async Task<IActionResult> GetEmployeeById(string id)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeDetailsByIdAsync(id);
                if (employee == null)
                {
                    return NotFound(new GeneralResponse
                    {
                        Success = false,
                        Data = "Employee not found."
                    });
                }

                var roles = await _userManager.GetRolesAsync(employee.User);
                var claims = await _userManager.GetClaimsAsync(employee.User);
                
                #region
                //var result = new EmployeeDetailsDto
                //{
                //    EmployeeId = employee.Id,
                //    Role = employee.Role,
                //    Salary = employee.Salary,
                //    Specialization = employee.Specialization,
                //    BranchId = employee.BranchId,

                //    User = new UserDto
                //    {
                //        Id = employee.User.Id,
                //        UserName = employee.User.UserName,
                //        Email = employee.User.Email,
                //        PhoneNumber = employee.User.PhoneNumber,
                //        FirstName = employee.User.FirstName,
                //        LastName = employee.User.LastName,
                //        DateOfBirth = employee.User.DateOfBirth,
                //        IsActive = employee.User.IsActive
                //    },

                //    Roles = roles.ToList(),
                //    Claims = claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList()
                //};
                #endregion


                var result = _mapper.Map<EmployeeDetailsDto>(employee);

                result.Roles = roles.ToList();
                result.Claims = claims.Select(c => _mapper.Map<ClaimDto>(c)).ToList();


                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = result
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



        [HttpPatch("employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] UpdateEmployeeDto dto)
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

                var updatedEmployee = await _employeeRepository.UpdateEmployeeAsync(id, dto);
                if (updatedEmployee == null)
                    return NotFound(new GeneralResponse { Success = false, Data = "Employee not found." });

                if (updatedEmployee == null || updatedEmployee.User == null || string.IsNullOrWhiteSpace(updatedEmployee.User.Email))
                {
                    return BadRequest(new GeneralResponse
                    {
                        Success = false,
                        Data = "Employee or email not found!"
                    });
                }

                //await _mediator.Publish(new UserRegisteredEvent(updatedEmployee.User.Email, "Heads Up", "Your Account Info Was Updated"));
                await _mediator.Publish(new UserRegisteredEvent("karamomari20112001@gmail.com", "Heads Up", "Your Account Info Was Updated"));

                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = "Employee updated successfully."
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






