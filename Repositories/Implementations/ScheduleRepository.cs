namespace UserGuard_API.Repositories.Implementations
{
    public class ScheduleRepository:IScheduleRepository
    {
        private readonly AcademyAPI _context;

        public ScheduleRepository(AcademyAPI context)
        {
            _context = context;
        }

        public async Task<Schedule?> AddExamScheduleAsync(ScheduleDto dto, string teacherId)
        {
            var schedule = new Schedule
            {
                Id = Guid.NewGuid().ToString(),
                CourseId = dto.CourseId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<List<Schedule>> GetSchedulesByCourseIdAsync(string courseId)
        {
            return await _context.Schedules
                .Where(s => s.CourseId == courseId)
                .ToListAsync();
        }
    }
}
