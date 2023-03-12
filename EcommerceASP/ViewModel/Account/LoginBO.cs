using System.ComponentModel.DataAnnotations;

namespace EcommerceASP.ViewModel.Account
{
    public class LoginBO
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}