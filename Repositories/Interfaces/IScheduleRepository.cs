namespace UserGuard_API.Repositories.Interfaces
{
    public interface IScheduleRepository
    {
        Task<Schedule?> AddExamScheduleAsync(ScheduleDto dto, string teacherId);
        Task<List<Schedule>> GetSchedulesByCourseIdAsync(string courseId);
    }
}
