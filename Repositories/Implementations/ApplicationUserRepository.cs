using Microsoft.AspNetCore.Http.HttpResults;

namespace UserGuard_API.Repositories.Implementations
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AcademyAPI context;

        public ApplicationUserRepository(UserManager<ApplicationUser> userManager,AcademyAPI context)
        {
            _userManager = userManager;
            this.context = context;
        }

        //public ApplicationUser GetWithDetilas(string currentUserId)
        //{
        //    await _userManager.
        //       .Include(u => u.Employee)
        //       .FirstOrDefaultAsync(u => u.Id == currentUserId);

        //}

        public ApplicationUser CreateApplicationUser(RegisterEmployeeDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                EmailConfirmed = true,
                IsActive = true,
                AccountCreatedAt = DateTime.UtcNow
            };
            return user;

        }
    }

}
