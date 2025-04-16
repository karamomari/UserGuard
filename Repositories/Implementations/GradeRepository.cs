namespace UserGuard_API.Repositories.Implementations
{
    public class GradeRepository:IGradeRepository
    {
        private readonly AcademyAPI _context;

        public GradeRepository(AcademyAPI context)
        {
            _context = context;
        }

        public async Task<Grade?> AddGradeAsync(GradeDto dto, string teacherId)
        {
            var grade = new Grade
            {
                Id = Guid.NewGuid().ToString(),
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Mark = dto.Mark
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return grade;
        }

        public async Task<GradeDto?> UpdateGradeAsync(GradeDto newgrade,string TeacherId)
        {

           var oldgrade=await _context.Grades.Include(g=>g.Course).FirstOrDefaultAsync(g => g.CourseId == newgrade.CourseId && g.StudentId == newgrade.StudentId);
            if (oldgrade == null)
                return null;
            if (oldgrade.Course.TeacherId != TeacherId)
            {
                return null;
            }
            oldgrade.Mark = newgrade.Mark;
            await _context.SaveChangesAsync();
            return newgrade;
        }


        public async Task<Grade?> GetGradeByStudentAndCourseAsync(string studentId, string courseId)
        {
            return await _context.Grades.FirstOrDefaultAsync(g => g.StudentId == studentId && g.CourseId == courseId);
        }
    }
}
