namespace UserGuard_API.DTO
{
    public class CourseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreditHours { get; set; }

        public string? TeacherId { get; set; }

       
    }
}
