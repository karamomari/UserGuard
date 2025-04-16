namespace UserGuard_API.DTO
{
    public class EmployeeDetailsDto
    {
      
            public string EmployeeId { get; set; }
            public string Role { get; set; }
            public decimal? Salary { get; set; }
            public string Specialization { get; set; }
            public string BranchId { get; set; }

        public UserDto User { get; set; }
        public List<string> Roles { get; set; }
        public List<ClaimDto> Claims { get; set; }

    }
}
