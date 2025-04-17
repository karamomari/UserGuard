namespace UserGuard_API.DTO
{
    public class BranchDetailsDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string ManagerName { get; set; }
        public List<EmployeeGenralDto> Employees { get; set; }
        public List<CourseDto> Courses { get; set; }

    }
}
