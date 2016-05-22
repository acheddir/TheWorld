using System.ComponentModel.DataAnnotations;

namespace TheWorld.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "is required")]
        [StringLength(1024, MinimumLength = 5, ErrorMessage = "must be at least 5 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "is required")]
        [EmailAddress(ErrorMessage = "must be an email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "is required")]
        [StringLength(1024, MinimumLength = 5, ErrorMessage = "must be at least 5 characters")]
        public string Message { get; set; }
    }
}