namespace UserGuard_API.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesByBranchIdAsync(string branchId);
        Task<Employee> GetEmployeeByUserIdAsync(string userId);
         Task<Employee> AddEmployee(RegisterEmployeeDto dto, string applicationUserId,string BranchId);

         Task<Employee> GetEmployeeByEmployeeIdAsync(string EmployeeId);

        Task<Employee?> UpdateEmployeeAsync(string id, UpdateEmployeeDto dto);
        Task<Employee> GetEmployeeDetailsByIdAsync(string id);
        Task<Employee> GetEmployeeDetailsByUserIdAsync(string id);
    }
}
