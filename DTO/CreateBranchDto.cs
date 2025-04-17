namespace UserGuard_API.DTO
{
    public class CreateBranchDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string? ManagerId { get; set; }
    }
}
