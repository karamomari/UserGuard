using Microsoft.EntityFrameworkCore;
using UserGuard_API.Repositories.Interfaces;

namespace UserGuard_API.Repositories.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AcademyAPI context;
        private readonly IEmployeeRepository _employeeRepository;
        public CourseRepository(AcademyAPI context, IEmployeeRepository _employeeRepository)
        {
            this.context = context;
            this._employeeRepository = _employeeRepository;
        }

        public async Task<Course> CreateCourseAsync(CourseDto courseDto, string AdminbranchId)
        {
            var course = new Course
            {
                Name = courseDto.Name,
                Description = courseDto.Description,
                CreditHours = courseDto.CreditHours,
                BranchId = AdminbranchId,
                TeacherId = courseDto?.TeacherId
            };
            await context.Courses.AddAsync(course);
            await context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateCourseAsync(CourseToUpdateDto courseToUpdate, string id)
        {
            var course = await context.Courses.FirstOrDefaultAsync(d => d.Id == id);
            if (course == null) return null;
            
            if (!string.IsNullOrEmpty(courseToUpdate.Name))
                course.Name = courseToUpdate.Name;

            if (!string.IsNullOrEmpty(courseToUpdate.Description))
                course.Description = courseToUpdate.Description;

            if (courseToUpdate.CreditHours.HasValue)
                course.CreditHours = courseToUpdate.CreditHours.Value;

            if (!string.IsNullOrEmpty(courseToUpdate.TeacherId))
                course.TeacherId = courseToUpdate.TeacherId;


            context.Courses.Update(course);
            await context.SaveChangesAsync();
            return course;
        }



        public async Task DeleteCourseAsync(Course course)
        {
            context.Courses.Remove(course);
            await context.SaveChangesAsync();
        }

        public async Task<Course> GetCourseByIdAsync(string id)
        {
            return await context.Courses.FirstOrDefaultAsync(d => d.Id == id);
        }


        public async Task<Course> FindByNameAsync(string Name)
        {
            return await context.Courses.FirstOrDefaultAsync(d => d.Name == Name);
        }

        public async Task<Course> GetCourseByNameAsync(string name)
        {
            return await context.Courses.FirstOrDefaultAsync(d => d.Name == name);
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await context.Courses.ToListAsync();
        }



        public async Task<List<Course>> GetCoursesForAdminAsync(string AdminbranchId)
        {
            return await context.Courses
                .Where(c => c.BranchId == AdminbranchId)
                .ToListAsync();
        }


        public async Task<Course> GetCourseByIdForAdminAsync(string courseId, string AdminbranchId)
        {
            return await context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId && c.BranchId == AdminbranchId);
        }


        public async Task<List<Course>> GetCoursesByTeacherIdAsync(string teacherId)
        {
            return await context.Courses
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentCourse>> GetAllStudentCoursesAsync()
        {
            return await context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .ToListAsync();
        }
    
    
        public async Task<List<Course>> GetCoursesByBranchId(string BranchId)
        {
           var courses= await context.Courses.Include(c => c.Branch).Where(co => co.BranchId == BranchId).ToListAsync();
            return courses;
        }

        public async Task<List<Course>> GetCoursesByBranchToSupperAdminAsync(string branchId)
        {
            return await context.Courses
                .Where(c => c.BranchId == branchId)
                .ToListAsync();
        }

    }

}
