using System.ComponentModel.DataAnnotations.Schema;

namespace UserGuard_API.Model
{
    public class MaintenanceTask
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TaskDescription { get; set; }
        public DateTime TaskDate { get; set; }

        public DateTime? TaskEnd { get; set; }

        public string Status { get; set; } = "Pending"; 
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }

        [ForeignKey("Technician")]
        public string EmployeeId { get; set; }
        public Employee Technician { get; set; }

        [ForeignKey("Branch")]
        public string BranchId { get; set; }
        public Branch Branch { get; set; }

        public List<MaintenanceTaskImage> Images { get; set; } = new();
    }

}
