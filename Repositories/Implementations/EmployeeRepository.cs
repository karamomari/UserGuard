
using Microsoft.EntityFrameworkCore;

namespace UserGuard_API.Repositories.Implementations
{

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AcademyAPI _dbContext;

        public EmployeeRepository(AcademyAPI dbContext)
        {
            _dbContext = dbContext;
        }
        //GetEmployeeDetailsByIdAsync
        public async Task<IEnumerable<Employee>> GetEmployeesByBranchIdAsync(string branchId)
        {
            return await _dbContext.Employees
                                   .Where(e => e.BranchId == branchId)
                                   .ToListAsync();
        }

        public async Task<Employee> GetEmployeeByUserIdAsync(string userId)
        {
            return await _dbContext.Employees
                                   .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
        }


        public async Task<Employee> GetEmployeeByEmployeeIdAsync(string EmployeeId)
        {
            return await _dbContext.Employees
                                   .FirstOrDefaultAsync(e => e.Id == EmployeeId);
        }


        public async Task<Employee> AddEmployee(RegisterEmployeeDto dto,string applicationUserId,string BranchId)
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid().ToString(),
                Role = dto.Role,
                Salary = dto.Salary,
                Specialization = dto.Specialization,
                BranchId = BranchId,
                ApplicationUserId = applicationUserId
            };

            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();

            return employee;
        }




        public async Task<Employee?> UpdateEmployeeAsync(string id, UpdateEmployeeDto dto)
        {
            var employee = await _dbContext.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return null;

            if (dto.Salary.HasValue)
                employee.Salary = dto.Salary.Value;

            if (!string.IsNullOrWhiteSpace(dto.Specialization))
                employee.Specialization = dto.Specialization;

            if (dto.IsActive.HasValue)
                employee.User.IsActive = dto.IsActive.Value;

            await _dbContext.SaveChangesAsync();
            return employee;
        }


        public async Task<Employee> GetEmployeeDetailsByIdAsync(string id)
        {
            return await _dbContext.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);

        }


        public async Task<Employee> GetEmployeeDetailsByUserIdAsync(string id)
        {
            var emp = await this.GetEmployeeByUserIdAsync(id);
            if (emp == null)
                return null;

            var detemp= await this.GetEmployeeDetailsByIdAsync(emp.Id);

            return detemp;

        }
    }
}