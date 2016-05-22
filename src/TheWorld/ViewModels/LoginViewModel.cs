using System.ComponentModel.DataAnnotations;

namespace TheWorld.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "is required")]
        [Display(Name = "Password")]
        public string Pwd { get; set; } 
    }
}