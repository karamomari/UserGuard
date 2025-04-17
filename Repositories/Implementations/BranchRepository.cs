using Microsoft.AspNetCore.Http.HttpResults;

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

        //to supper Admin
        public async Task<Branch> GetBranchWithManagerAsync(string branchId)
        {
            return await _dbContext.Branches
                .Include(b => b.Manager)  
                .FirstOrDefaultAsync(b => b.Id == branchId);
        }

        public async Task<List<Branch>> GetAllBranch()
        {
          return await _dbContext.Branches.Include(m=>m.Manager).ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByBranchAsync(string branchId)
        {
            return await _dbContext.Courses
                .Where(c => c.BranchId == branchId)  
                .ToListAsync();
        }


        public async Task<bool> UpdateBranchAsync(string id, Branch updatedBranch)
        {
            var branch = await _dbContext.Branches.FindAsync(id);

            if (branch == null)
                return false;

            if (!string.IsNullOrWhiteSpace(updatedBranch.Name))
                branch.Name = updatedBranch.Name;

            if (!string.IsNullOrWhiteSpace(updatedBranch.Location))
                branch.Location = updatedBranch.Location;

            if (!string.IsNullOrWhiteSpace(updatedBranch.ManagerId))
                branch.ManagerId = updatedBranch.ManagerId;


            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Branch> AddBranchAsync(Branch branch)
        {
            branch.Id = Guid.NewGuid().ToString(); 
            await _dbContext.Branches.AddAsync(branch);
            await _dbContext.SaveChangesAsync();
            return branch;
        }


    }

}
