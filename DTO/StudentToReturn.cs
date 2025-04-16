namespace UserGuard_API.DTO
{
    public class StudentToReturn
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string BranchName { get; set; }
        public List<string> Courses { get; set; }

    }
}
