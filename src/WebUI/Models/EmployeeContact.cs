using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    /// <summary>
    /// Represents an employee contact in the system
    /// </summary>
    public class EmployeeContact
    {
        /// <summary>
        /// Gets or sets the unique identifier for the contact
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the full name of the employee
        /// </summary>
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number of the employee
        /// </summary>
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Phone number must be a valid international format")]
        public string PersonalPhone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the CUG phone number of the employee
        /// </summary>
        [Required(ErrorMessage = "CUG Phone number is required")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Phone number must be a valid international format")]
        public string WorkPhone { get; set; } = null!;

        /// <summary>
        /// Gets or sets the email address of the employee
        /// </summary>
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the job title of the employee
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the company name
        /// </summary>
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public string Company { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the LinkedIn profile URL
        /// </summary>
        [Url(ErrorMessage = "LinkedIn URL must be a valid URL")]
        [StringLength(200, ErrorMessage = "LinkedIn URL cannot exceed 200 characters")]
        public string? LinkedIn { get; set; }
    }
} 