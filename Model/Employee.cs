

namespace UserGuard_API.Model
{
  public class Employee 
    {
        [Key]
        public string Id { get; set; }

        //public string Phone { get; set; }
        public string Role { get; set; } 
  
        public decimal? Salary { get; set; }
        public string Specialization { get; set; }

        [ForeignKey("Branch")]
        public string BranchId { get; set; }
        public Branch Branch { get; set; }

        // ارتباط بـ ApplicationUser
        [ForeignKey("User")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
    }

}



