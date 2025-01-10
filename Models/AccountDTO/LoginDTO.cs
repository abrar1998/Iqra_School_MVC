using System.ComponentModel.DataAnnotations;

namespace SchoolProj.Models.AccountDTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Enter your user name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        public string UserPassword { get; set; }
        public bool RememberMe { get; set; }
    }
}
