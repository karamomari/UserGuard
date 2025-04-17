namespace UserGuard_API.Repositories.Interfaces
{
    public interface IBranchRepository
    {
        Task<Branch> GetBranchByIdAsync(string branchId);
        Task<List<Branch>> GetAllBranch();
        Task<Branch> GetBranchWithManagerAsync(string branchId);
        Task<List<Course>> GetCoursesByBranchAsync(string branchId);

        Task<bool> UpdateBranchAsync(string id, Branch updatedBranch);

        Task<Branch> AddBranchAsync(Branch branch);
    }
}
