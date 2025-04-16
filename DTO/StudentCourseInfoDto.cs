namespace UserGuard_API.DTO
{
    public class StudentCourseInfoDto
    {
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int CreditHours { get; set; }
        public string TeacherName { get; set; }
        public double? Mark { get; set; }
    }
}
