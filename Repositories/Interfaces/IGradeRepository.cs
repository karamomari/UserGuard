namespace UserGuard_API.Repositories.Interfaces
{
    public interface IGradeRepository
    {
        Task<Grade?> AddGradeAsync(GradeDto dto, string teacherId);
        Task<GradeDto?> UpdateGradeAsync(GradeDto newgrade, string TeacherId);
        Task<Grade?> GetGradeByStudentAndCourseAsync(string studentId, string courseId);
    }
}
