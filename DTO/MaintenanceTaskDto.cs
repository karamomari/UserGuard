namespace UserGuard_API.DTO
{
    public class MaintenanceTaskDto
    {
        public string TaskDescription { get; set; }
        public DateTime TaskDate { get; set; }
        public DateTime? TaskEnd { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; } // في حال بدنا نعرضها
        public string EmployeeId { get; set; } // اللي رح ينفذ المهمة
    }
}
