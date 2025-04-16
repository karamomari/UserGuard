namespace UserGuard_API.Repositories.Interfaces
{
    public interface IBranchRepository
    {
        Task<Branch> GetBranchByIdAsync(string branchId);
    }
}
