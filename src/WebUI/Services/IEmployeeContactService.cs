using WebUI.Models;

namespace WebUI.Services
{
    public interface IEmployeeContactService
    {
        Task<ApiResponse<List<EmployeeContact>>> GetContactsAsync(string? phone = null, string? startDate = null, string? endDate = null);
        Task<ApiResponse<EmployeeContact>> GetContactByPhoneAsync(string phone);
        Task<ApiResponse<byte[]>> GetQRCodeAsync(string phone);
        Task<ApiResponse<EmployeeContact>> CreateContactAsync(EmployeeContact contact);
        Task<ApiResponse<byte[]>> GetVCardAsync(string phone);
        Task<ApiResponse<bool>> AddContactAsync(EmployeeContact contact);
    }
} 