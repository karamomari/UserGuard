using UserGuard_API.Model;

namespace UserGuard_API.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> Add(StudentToCreateDto obj, string BranchId, string ApplicationUserId);
        Task<Student> Update(StudentToUpdateDto obj, string studentId);  
        Task<Student> Delete(string Id);
        Task<Student> GetById(string id);
        Task<List<Student>> GetAll();
        Task<Student> GetByIdWithDetails(string id);

        Task<List<StudentToReturn>> GetAllStudentsByBranchId(string BranchId);

        Task<Student> GetByIdIfInSameBranch(string studentId, string branchId);

        Task<bool> RegisterStudentInCourse(string studentId, string courseName);

        Task<Student> GetStudentByAppUserIdAsync(string userId);
        Task<List<StudentCourseInfoDto>> GetStudentCoursesWithGradesAsync(string studentId);

        Task<List<StudentInfoDto>> GetStudentsInSameCourses(string studentId);
    }
}
