namespace UserGuard_API.Model
{
    [Table("MaintenanceTaskImages")]
    public class MaintenanceTaskImage
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ImageUrl { get; set; }

        [ForeignKey("MaintenanceTask")]
        public string MaintenanceTaskId { get; set; }
        public MaintenanceTask MaintenanceTask { get; set; }
    }

}
