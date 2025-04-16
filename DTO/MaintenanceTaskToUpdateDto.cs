namespace UserGuard_API.DTO
{
    public class MaintenanceTaskToUpdateDto
    {
        public string TaskDescription { get; set; }
        public DateTime TaskDate { get; set; }
        public DateTime? TaskEnd { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public string EmployeeId { get; set; }
    }
}
