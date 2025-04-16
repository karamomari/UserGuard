namespace UserGuard_API.Model
{
    public class StudentCourse
    {

        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Student")]
        public string StudentId { get; set; }
        public Student Student { get; set; }

        [ForeignKey("Course")]
        public string CourseId { get; set; }
        public Course Course { get; set; }

    }
}
