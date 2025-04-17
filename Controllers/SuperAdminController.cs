

namespace UserGuard_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy ="SupperAdminPolicy")]

    public class SuperAdminController : ControllerBase
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
        private readonly IBranchRepository branchRepository;
        private readonly IExcelExportService excelExportService;

        public SuperAdminController(IEmployeeRepository employeeRepository,
          UserManager<ApplicationUser> userManager, AcademyAPI context,
          IApplicationUserRepository applicationUserRepository, RoleManager<IdentityRole> role
          , ICourseRepository courseRepository, IMaintenanceTaskRepository maintenanceTaskRepository
          , IMediator mediator, IMapper mapper,IBranchRepository branchRepository,IExcelExportService excelExportService)
        {
            _employeeRepository = employeeRepository;
            _userManager = userManager;
            this._context = context;
            this.applicationUserRepository = applicationUserRepository;
            this._roleManager = role;
            this.courseRepository = courseRepository;
            this._taskRepo = maintenanceTaskRepository;
            this._mediator = mediator;
            _mapper = mapper;
            this.branchRepository = branchRepository;
            this.excelExportService = excelExportService;
        }





        [HttpGet("GetAllBranch")]
        public async Task <IActionResult> GetAllBranch()
        {
            try
            {
                var branches = await branchRepository.GetAllBranch();

                var branchDtos = _mapper.Map<List<BranchListDto>>(branches);

                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = branchDtos
                });
            }
            catch (Exception e)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = e.Message });
            }
        }
     

        [HttpGet("GetBranchDetails/{branchId}")]
        public async Task<IActionResult> GetBranchDetailsAsync(string branchId)
        {
            try
            {
                var branch = await branchRepository.GetBranchByIdAsync(branchId);

                if (branch == null)
                {
                    return BadRequest(new GeneralResponse { Success = false, Data = "Invalid Branch id " });
                }
                var courses = await courseRepository.GetCoursesByBranchToSupperAdminAsync(branchId);

                var employees = await _employeeRepository.GetEmployeesByBranchIdAsync(branchId);

                var branchDto = _mapper.Map<BranchDetailsDTO>(branch);
                branchDto.Courses = _mapper.Map<List<CourseDto>>(courses);
                branchDto.Employees = _mapper.Map<List<EmployeeGenralDto>>(employees);

                return Ok(new GeneralResponse { Success = true, Data = branchDto });
            }
            catch (Exception e)
            {
                return BadRequest(new GeneralResponse { Success = false, Data = e.Message });
            }
        }






        [HttpGet("export-excel/{branchId}")]
        public async Task<IActionResult> ExportBranchExcel(string branchId)
        {
            var branch = await branchRepository.GetBranchByIdAsync(branchId);
            if (branch == null) return NotFound();

            var courses = await courseRepository.GetCoursesByBranchToSupperAdminAsync(branchId);
            var employees = await _employeeRepository.GetEmployeesByBranchIdAsync(branchId);

            var dto = _mapper.Map<BranchDetailsDTO>(branch);
            dto.Courses = _mapper.Map<List<CourseDto>>(courses);
            dto.Employees = _mapper.Map<List<EmployeeGenralDto>>(employees);

            var excelBytes = excelExportService.ExportBranchDetailsToExcel(dto);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BranchDetails.xlsx");
        }



        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateBranch(string id, [FromBody] UpdateBranchDto dto)
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
                var updatedBranch = _mapper.Map<Branch>(dto);

                var success = await branchRepository.UpdateBranchAsync(id, updatedBranch);

                if (!success)
                    return NotFound(new GeneralResponse { Success = false, Data = "Branch not found" });

                return Ok(new GeneralResponse { Success = true, Data = "Branch updated successfully" });
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


        [HttpPost("CreateBranch")]
        public async Task<IActionResult> CreateBranch([FromBody] CreateBranchDto dto)
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
                var branch = _mapper.Map<Branch>(dto);

                var createdBranch = await branchRepository.AddBranchAsync(branch);

                var responseDto = _mapper.Map<CreateBranchDto>(createdBranch);


                return Ok(new GeneralResponse
                {
                    Success = true,
                    Data = responseDto
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
