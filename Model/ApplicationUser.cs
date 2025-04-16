

namespace UserGuard_API.Model
{
    public class ApplicationUser: IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? AccountCreatedAt { get; set; }
        public string? LastTokenGenerated { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }

        // رابطه مع Employee (موظف، إذا كان موجودًا)
        public Employee Employee { get; set; }

        public Student Student { get; set; }


    }
}
