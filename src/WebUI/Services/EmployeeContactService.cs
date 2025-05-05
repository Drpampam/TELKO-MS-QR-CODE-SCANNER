using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WebUI.Models;

namespace WebUI.Services
{
    public class EmployeeContactService : IEmployeeContactService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<EmployeeContactService> _logger;

        public EmployeeContactService(HttpClient httpClient, IConfiguration configuration, ILogger<EmployeeContactService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException("ApiSettings:BaseUrl is not configured");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                throw new ArgumentException("API base URL is not configured", nameof(configuration));
            }

            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<ApiResponse<List<EmployeeContact>>> GetContactsAsync(string? phone = null, string? startDate = null, string? endDate = null)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/api/v1/contacts/all-contacts", null);
                response.EnsureSuccessStatusCode();
                
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<EmployeeContact>>>();
                return result ?? new ApiResponse<List<EmployeeContact>> { Success = false, Message = "Failed to get contacts" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contacts");
                return new ApiResponse<List<EmployeeContact>> { Success = false, Message = $"Error getting contacts: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<EmployeeContact>> GetContactByPhoneAsync(string phone)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v1/contacts/{phone}/contact-details");
                response.EnsureSuccessStatusCode();
                
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeContact>>();
                return result ?? new ApiResponse<EmployeeContact> { Success = false, Message = "Failed to get contact" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact for phone: {Phone}", phone);
                return new ApiResponse<EmployeeContact> { Success = false, Message = $"Error getting contact: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<byte[]>> GetQRCodeAsync(string phone)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/api/v1/qrcode/{phone}/qrcode", null);
                response.EnsureSuccessStatusCode();
                
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<byte[]>>();
                return result ?? new ApiResponse<byte[]> { Success = false, Message = "Failed to get QR code" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting QR code for phone: {Phone}", phone);
                return new ApiResponse<byte[]> { Success = false, Message = $"Error getting QR code: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<EmployeeContact>> CreateContactAsync(EmployeeContact contact)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/v1/contacts/create-contact", contact);
                response.EnsureSuccessStatusCode();
                
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeContact>>();
                return result ?? new ApiResponse<EmployeeContact> { Success = false, Message = "Failed to create contact" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact");
                return new ApiResponse<EmployeeContact> { Success = false, Message = $"Error creating contact: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<byte[]>> GetVCardAsync(string phone)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v1/contacts/{phone}/contact.vcf");
                response.EnsureSuccessStatusCode();
                
                var bytes = await response.Content.ReadAsByteArrayAsync();
                return new ApiResponse<byte[]> { Success = true, Data = bytes };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vCard for phone: {Phone}", phone);
                return new ApiResponse<byte[]> { Success = false, Message = $"Error getting vCard: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> AddContactAsync(EmployeeContact contact)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/v1/contacts", contact);
                response.EnsureSuccessStatusCode();
                return new ApiResponse<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding contact");
                return new ApiResponse<bool> { Success = false, Message = $"Error adding contact: {ex.Message}" };
            }
        }
    }
} 