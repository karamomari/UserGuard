namespace UserGuard_API.DTO
{
    public class UpdateBranchDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string? ManagerId { get; set; }
    }
}
