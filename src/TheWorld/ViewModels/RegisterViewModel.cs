using System.ComponentModel.DataAnnotations;

namespace TheWorld.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "is required")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "must be at least 5 characters")]
        public string Username { get; set; }
        [Required(ErrorMessage = "is required")]
        [EmailAddress(ErrorMessage = "must be an email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "is required")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "must be a phone number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "is required")]
        [Display(Name = "Password")]
        public string Pwd { get; set; }
    }
}