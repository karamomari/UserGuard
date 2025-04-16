namespace UserGuard_API.DTO
{
    public class MaintenanceTaskDtoResponse
    {
        public string TaskDescription { get; set; }
        public DateTime TaskDate { get; set; }
        public DateTime? TaskEnd { get; set; }
        public string? Notes { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }
}
