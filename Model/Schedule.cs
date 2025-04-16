using System.ComponentModel.DataAnnotations.Schema;

namespace UserGuard_API.Model
{
    public class Schedule
    {
        [Key]
        public string Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
     

        [ForeignKey("Course")]
        public string CourseId { get; set; }

        public Course Course { get; set; }
    }

}
