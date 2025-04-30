using ConfigurationService.Domain.Common;

namespace Domain.Entities
{
    public class EmployeeContact : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string LinkedIn { get; set; } = string.Empty;
    }
}
