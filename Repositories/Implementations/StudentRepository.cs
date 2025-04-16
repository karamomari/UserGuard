

namespace UserGuard_API.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AcademyAPI context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICourseRepository courseRepository;

        public StudentRepository(AcademyAPI context, UserManager<ApplicationUser> userManager, ICourseRepository courseRepository)
        {
            this.context = context;
            this.userManager = userManager;
            this.courseRepository = courseRepository;
        }

        public async Task<Student> Add(StudentToCreateDto obj, string BranchId, string ApplicationUserId)
        {


            var student = new Student
            {
                ApplicationUserId = ApplicationUserId,
                BranchId = BranchId,
                FullName = obj.FullName,
                StudentCourses = new List<StudentCourse>()
            };

            foreach (var courseName in obj.CourseNames.Distinct())
            {
                var course = await courseRepository.FindByNameAsync(courseName);
                if (course != null)
                {
                    student.StudentCourses.Add(new StudentCourse { CourseId = course.Id });
                }
            }

            context.Students.Add(student);
            await context.SaveChangesAsync();
            return student;
        }

        public async Task<Student> Delete(string Id)
        {
            var obj = await this.GetByIdWithDetails(Id);
            if (obj == null)
                return null;

            if (obj.StudentCourses != null)
                context.StudentCourses.RemoveRange(obj.StudentCourses);

            // احذف حساب المستخدم المرتبط فيه
            if (obj.ApplicationUser != null)
                context.Users.Remove(obj.ApplicationUser);

            context.Students.Remove(obj);
            await context.SaveChangesAsync();
            return obj;
        }

        public async Task<Student> Update(StudentToUpdateDto obj, string studentId)
        {
            var student = await this.GetByIdWithDetails(studentId);
            if (student == null)
                return null;

            if (obj.FullName != null)
            {
                student.FullName = obj.FullName;
            }

            if (obj.CourseNames != null)
            {
                student.StudentCourses.Clear();
                foreach (var courseName in obj.CourseNames)
                {
                    var course = await courseRepository.FindByNameAsync(courseName);
                    if (course != null)
                    {
                        student.StudentCourses.Add(new StudentCourse { Course = course, Student = student });
                    }
                }
            }

            context.Update(student);
            await context.SaveChangesAsync();
            return student;
        }


        public async Task<bool> RegisterStudentInCourse(string studentId, string courseName)
        {
            var student = await context.Students
                .Include(s => s.StudentCourses)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return false;

            var course = await courseRepository.FindByNameAsync(courseName);
            if (course == null)
                return false;

            var alreadyRegistered = student.StudentCourses.Any(sc => sc.CourseId == course.Id);
            if (alreadyRegistered)
                return false;

            var studentCourse = new StudentCourse
            {
                StudentId = studentId,
                CourseId = course.Id
            };

            context.StudentCourses.Add(studentCourse);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Student>> GetAll()
        {
            return await context.Students.ToListAsync();
        }

        public async Task<Student> GetById(string id)
        {
            return await context.Students.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Student> GetByIdWithDetails(string id)
        {
            return await context.Students.Include(s => s.Branch)
                                           .Include(s => s.StudentCourses)
                                         .ThenInclude(sc => sc.Course)
                                         .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<StudentToReturn>> GetAllStudentsByBranchId(string branchId)
        {
            var students = await context.Students
        .Where(st => st.BranchId == branchId)
        .Include(s => s.StudentCourses)
            .ThenInclude(sc => sc.Course)
        .Include(s => s.Branch)
        .ToListAsync();

            return students.Select(s => new StudentToReturn
            {
                Id = s.Id,
                FullName = s.FullName,
                BranchName = s.Branch?.Name,
                Courses = s.StudentCourses.Select(sc => sc.Course.Name).ToList()
            }).ToList();
        }

        public async Task<Student> GetByIdIfInSameBranch(string studentId, string branchId)
        {
            return await context.Students
                                .Include(s => s.StudentCourses)
                                    .ThenInclude(sc => sc.Course)
                                .FirstOrDefaultAsync(s => s.Id == studentId && s.BranchId == branchId);
        }

        public async Task<Student> GetStudentByAppUserIdAsync(string userId)
        {
            var student = await context.Students.Where(s => s.ApplicationUserId == userId).FirstOrDefaultAsync();
            if (student == null)
                return null;
            return student;
        }

        public async Task<List<StudentCourseInfoDto>> GetStudentCoursesWithGradesAsync(string studentId)
        {
            return await context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Include(sc => sc.Course)
                    .ThenInclude(c => c.Teacher)
                        .ThenInclude(t => t.User) // التأكد من أن Teacher مرتبط بـ User
                .Select(sc => new StudentCourseInfoDto
                {
                    CourseName = sc.Course.Name,
                    Description = sc.Course.Description,
                    CreditHours = sc.Course.CreditHours,

                    // دمج اسم المدرس
                    // TeacherName = sc.Course.Teacher.User.FirstName + " " + sc.Course.Teacher.User.LastName,
                    TeacherName = sc.Course.Teacher != null && sc.Course.Teacher.User != null
                     ? sc.Course.Teacher.User.FirstName + " " + sc.Course.Teacher.User.LastName
                     : "غير معروف",

                    // جلب العلامة من الـ Grade
                    Mark = context.Grades
                        .Where(g => g.StudentId == studentId && g.CourseId == sc.CourseId)
                        .Select(g => g.Mark)
                        .FirstOrDefault() // إرجاع أول نتيجة أو صفر إذا مفيش علامات
                })
                .ToListAsync();
        }


        public async Task<List<StudentInfoDto>> GetStudentsInSameCourses(string studentId)
        {
            var studentsInSameCourses = await context.Database
                 .SqlQuery<StudentInfoDto>(
                     $"SELECT * FROM dbo.GetColleaguesInSameCourses({studentId})"
                 )
                 .ToListAsync();
            return studentsInSameCourses;

        }





    }

}
