namespace UserGuard_API.Model
{
    public class Branch
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public List<Course> Courses { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Student> Students { get; set; }

        public List<MaintenanceTask> MaintenanceTasks { get; set;}

    }

}
