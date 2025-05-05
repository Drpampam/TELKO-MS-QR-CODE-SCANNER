namespace WebUI.Models
{
    /// <summary>
    /// Represents a standardized API response
    /// </summary>
    /// <typeparam name="T">The type of data contained in the response</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the response message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the response data
        /// </summary>
        public T? Data { get; set; }
    }
} 