namespace UserGuard_API.Repositories.Implementations
{
    public class BranchRepository : IBranchRepository
    {
        private readonly AcademyAPI _dbContext;

        public BranchRepository(AcademyAPI dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Branch> GetBranchByIdAsync(string branchId)
        {
            return await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == branchId);
        }
    }

}
