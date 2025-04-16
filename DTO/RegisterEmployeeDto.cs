namespace UserGuard_API.DTO
{
    public class RegisterEmployeeDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        // بيانات الموظف
        public string Role { get; set; }
        public decimal? Salary { get; set; }
        public string Specialization { get; set; }
        public string BranchId { get; set; }
    }
}
