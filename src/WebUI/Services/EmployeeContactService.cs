using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WebUI.DTOs;

namespace WebUI.Services
{
    public class EmployeeContactService : IEmployeeContactService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;
        private readonly ILogger<EmployeeContactService> _logger;

        public EmployeeContactService(HttpClient httpClient, IConfiguration configuration, ILogger<EmployeeContactService> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException("ApiSettings:BaseUrl is not configured");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                throw new ArgumentException("API base URL is not configured", nameof(configuration));
            }

            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DTOs.ApiResponse<PaginationResult<DTOs.EmployeeContact>>> GetContactsAsync(string? phone , string? startDate, string? endDate )
        {
            try
            {
                var response = await _httpClient.PostAsync($"/api/v1/contacts/all-contacts", null);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<DTOs.ApiResponse<PaginationResult<DTOs.EmployeeContact>>>();
                if(result.Data == null)
                {
                    return new DTOs.ApiResponse<PaginationResult<DTOs.EmployeeContact>>
                    {
                        ResponseCode = "99",
                        Message = "Failed to get contacts"
                    };
                }

                var res = new DTOs.ApiResponse<PaginationResult<DTOs.EmployeeContact>>()
                {
                    ResponseCode = result.ResponseCode,
                    Message = result.Message,
                    Success = true,
                    Data = result.Data
                };
                return res;
             
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contacts");
                return new DTOs.ApiResponse<PaginationResult<DTOs.EmployeeContact>>
                {
                    ResponseCode = "99",
                    Message = $"Error getting contacts: {ex.Message}"
                };
            }
        }
        

        public async Task<DTOs.ApiResponse<EmployeeContact>> GetContactByPhoneAsync(string phone)
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

        public async Task<ApiResponse<byte[]>> GetQRCodeAsyncV2(string phone)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                // Set BaseAddress if not set
                if (client.BaseAddress == null)
                {
                    client.BaseAddress = new Uri(_baseUrl); // Base URL from configuration
                }

                var response = await client.PostAsync($"api/v1/QRCode/{phone}/qrcode", null);

                if (response.IsSuccessStatusCode)
                {
                    // ? Success — read image as bytes
                    var byteArray = await response.Content.ReadAsByteArrayAsync();
                    return new ApiResponse<byte[]>
                    {
                        Success = true,
                        Data = byteArray
                    };
                }
                else
                {
                    // ? Failure — read error JSON
                    var errorJson = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<string>>(errorJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new ApiResponse<byte[]>
                    {
                        Success = false,
                        Message = errorResponse?.Message ?? "Failed to generate QR code"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<byte[]>
                {
                    Success = false,
                    Message = ex.Message
                };
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