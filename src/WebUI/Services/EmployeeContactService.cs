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
            _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl ?? "http://localhost:5156");
        }

        public async Task<List<EmployeeContact>> GetAllContactsAsync()
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/v1/contacts/all-contacts", new { });
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<EmployeeContact>>>();
                return result?.Data ?? new List<EmployeeContact>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting contacts: {ex.Message}");
                return new List<EmployeeContact>();
            }
        }

        public async Task<EmployeeContact?> GetContactByPhoneAsync(string phone)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v1/contacts/{phone}/contact-details");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<BaseResponse<EmployeeContact>>();
                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting contact: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]?> GetQRCodeAsync(string phone)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/api/v1/qrcode/{phone}/qrcode", null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<BaseResponse<byte[]>>();
                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting QR code: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddContactAsync(EmployeeContact contact)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/v1/contacts", contact);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
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

        public Task<ApiResponse<List<EmployeeContact>>> GetContactsAsync(string? phone = null, string? startDate = null, string? endDate = null)
        {
            throw new NotImplementedException();
        }

        Task<ApiResponse<EmployeeContact>> IEmployeeContactService.GetContactByPhoneAsync(string phone)
        {
            throw new NotImplementedException();
        }

        Task<ApiResponse<byte[]>> IEmployeeContactService.GetQRCodeAsync(string phone)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<EmployeeContact>> CreateContactAsync(EmployeeContact contact)
        {
            throw new NotImplementedException();
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