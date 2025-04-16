namespace UserGuard_API.Repositories.Interfaces
{
    public interface IApplicationUserRepository
    {
       ApplicationUser CreateApplicationUser(RegisterEmployeeDto dto);
    }
}
