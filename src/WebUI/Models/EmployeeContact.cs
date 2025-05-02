using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class EmployeeContact
    {
        public required string Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Display(Name = "Phone")]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Title")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        public required string Company { get; set; }

        [Required(ErrorMessage = "LinkedIn URL is required")]
        [Url(ErrorMessage = "Invalid URL")]
        [Display(Name = "LinkedIn URL")]
        public required string LinkedIn { get; set; }
    }
} 