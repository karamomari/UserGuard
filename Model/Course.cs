

namespace UserGuard_API.Model
{
    public class Course
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreditHours { get; set; }
        [ForeignKey("Teacher")]
        public string TeacherId { get; set; } 

        public Employee Teacher { get; set; }


        [ForeignKey("Branch")]
        public string BranchId { get; set; } 
        public Branch Branch { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
    }

}
