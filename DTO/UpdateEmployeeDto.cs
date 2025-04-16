namespace UserGuard_API.DTO
{
    public class UpdateEmployeeDto
    {
        public decimal? Salary { get; set; }
        public string? Specialization { get; set; }
        public bool? IsActive { get; set; }

        public string? Role { get; set; }
    }
}
