using System.ComponentModel.DataAnnotations;

namespace TheWorld.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Pwd { get; set; } 
    }
}