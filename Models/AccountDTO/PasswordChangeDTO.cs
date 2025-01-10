using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models.AccountDTO
{
    public class PasswordChangeDTO
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[@#]).{5,}$",
          ErrorMessage = "Password must contain at least one uppercase letter and one special character (@ or #).")]
        public string NewPassword { get; set; }
    }
}
