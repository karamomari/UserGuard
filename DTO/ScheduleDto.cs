using System.ComponentModel;
using System.Text.Json.Serialization;

namespace UserGuard_API.DTO
{
    public class ScheduleDto
    {

        [Required]
        public string CourseId { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan StartTime { get; set; }

        [Required]
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan EndTime { get; set; }
    }
}
