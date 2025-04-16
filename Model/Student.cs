using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace UserGuard_API.Model
{
    public class Student
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("Branch")]
        public string BranchId { get; set; }
        public Branch Branch {  get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }

    }
}
