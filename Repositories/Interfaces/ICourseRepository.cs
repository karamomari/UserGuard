namespace UserGuard_API.Repositories.Interfaces
{
    public interface ICourseRepository
    {
      
        Task<Course> CreateCourseAsync(CourseDto courseDto,string AdminbranchId);
       
        Task<Course> UpdateCourseAsync(CourseToUpdateDto courseDto,string id);
        Task DeleteCourseAsync(Course course);
        Task<Course> GetCourseByIdAsync(string id);
        Task<Course> FindByNameAsync(string Name);
        Task<Course> GetCourseByNameAsync(string name);

        Task<List<Course>> GetCoursesForAdminAsync(string AdminbranchId);

        Task<List<Course>> GetAllAsync();
        Task<List<Course>> GetCoursesByBranchToSupperAdminAsync(string branchId);
        Task<IEnumerable<StudentCourse>> GetAllStudentCoursesAsync();

        Task<List<Course>> GetCoursesByTeacherIdAsync(string teacherId);

        Task<List<Course>> GetCoursesByBranchId(string BranchId);
    }

}
