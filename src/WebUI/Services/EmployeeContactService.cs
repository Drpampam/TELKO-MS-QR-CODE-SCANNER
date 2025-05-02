using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using WebUI.Models;

namespace WebUI.Services
{
    public class EmployeeContactService : IEmployeeContactService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        public EmployeeContactService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl ?? "http://localhost:5159");
        }

        public async Task<ApiResponse<List<EmployeeContact>>> GetContactsAsync(string? phone = null, string? startDate = null, string? endDate = null)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/v1/contacts/all-contacts", new { phone, startDate, endDate });
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiResponse<List<EmployeeContact>>>() ?? new ApiResponse<List<EmployeeContact>> { IsSuccess = false, Message = "Failed to deserialize response" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<EmployeeContact>> { IsSuccess = false, Message = $"Error getting contacts: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<EmployeeContact>> GetContactByPhoneAsync(string phone)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v1/contacts/{phone}/contact-details");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeContact>>() ?? new ApiResponse<EmployeeContact> { IsSuccess = false, Message = "Failed to deserialize response" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EmployeeContact> { IsSuccess = false, Message = $"Error getting contact: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<byte[]>> GetQRCodeAsync(string phone)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v1/qrcode/{phone}/qrcode");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiResponse<byte[]>>() ?? new ApiResponse<byte[]> { IsSuccess = false, Message = "Failed to deserialize response" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<byte[]> { IsSuccess = false, Message = $"Error getting QR code: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<EmployeeContact>> CreateContactAsync(EmployeeContact contact)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/v1/contacts", contact);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeContact>>() ?? new ApiResponse<EmployeeContact> { IsSuccess = false, Message = "Failed to deserialize response" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EmployeeContact> { IsSuccess = false, Message = $"Error creating contact: {ex.Message}" };
            }
        }

        public async Task<bool> AddContactAsync(EmployeeContact contact)
        {
            try
            {
                var request = new
                {
                    fullName = contact.FullName,
                    phone = contact.Phone,
                    email = contact.Email,
                    title = contact.Title,
                    company = contact.Company,
                    linkedIn = contact.LinkedIn
                };

                var response = await _httpClient.PostAsJsonAsync("/api/v1/Contacts", request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<BaseResponse<string>>();
                    Console.WriteLine($"Error adding contact: {errorResponse?.Message}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error adding contact: {ex.Message}");
                return false;
            }
        }

        public async Task<byte[]?> GetVCardAsync(string phone)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v1/contacts/{phone}/contact.vcf");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error getting vCard: {ex.Message}");
                return null;
            }
        }
    }

    public class BaseResponse<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
    }

    public class PaginationResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
} 