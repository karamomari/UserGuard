namespace UserGuard_API.Model
{
    public class Grade
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("Student")]
        public string StudentId { get; set; }
        public Student Student { get; set; }

        [ForeignKey("Course")]
        public string CourseId { get; set; }
        public Course Course { get; set; }

        public double Mark { get; set; }
    }
}
