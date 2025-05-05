using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    /// <summary>
    /// Represents the configuration settings for the API
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// Gets or sets the base URL of the API
        /// </summary>
        [Required(ErrorMessage = "API Base URL is required")]
        [Url(ErrorMessage = "API Base URL must be a valid URL")]
        public string BaseUrl { get; set; } = string.Empty;
    }
} 