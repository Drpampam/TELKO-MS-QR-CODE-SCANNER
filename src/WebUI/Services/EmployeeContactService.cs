using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WebUI.ApiClients;
using WebUI.DTOs;
using WebUI.DTOs.ContactRequestDtos;

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

        public async Task<DTOs.ApiResponse<PaginationResult<DTOs.EmployeeContact>>> GetContactsAsync(GetAllContacts filter)
        {
            const string FailureCode = "99";

            var result = await _httpClient.PostJsonAsync<GetAllContacts, DTOs.ApiResponse<PaginationResult<DTOs.EmployeeContact>>>(
                "api/v1/contacts/all-contacts",
                filter,
                _logger);

            if (result == null || result.Data == null)
            {
                return new DTOs.ApiResponse<PaginationResult<DTOs.EmployeeContact>>
                {
                    ResponseCode = FailureCode,
                    Message = "Failed to get contacts",
                    Success = false
                };
            }

            result.Success = true;
            return result;
        }

        public async Task<DTOs.ApiResponse<EmployeeContact>> GetContactByPhoneAsync(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                {
                    return new ApiResponse<EmployeeContact> { Success = false, Message = "Phone number cannot be empty" };
                }

                var encodedPhone = NormalizePhoneNumber(phone); // This handles spaces, +, etc.
                var response = await _httpClient.GetAsync($"api/v1/contacts/{encodedPhone}/contact-details");
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
                var encodedPhone = Uri.EscapeDataString(phone); // This handles spaces, +, etc.
                var response = await _httpClient.PostAsync($"api/v1/qrcode/{encodedPhone}/qrcode", null);
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

                var normalizedPhone = NormalizePhoneNumber(phone);

                var response = await client.PostAsync($"api/v1/QRCode/{normalizedPhone}/qrcode", null);

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
                var response = await _httpClient.PostAsJsonAsync("api/v1/contacts/create-contact", contact);
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
                var encodedPhone = Uri.EscapeDataString(phone); // This handles spaces, +, etc.
                var response = await _httpClient.GetAsync($"api/v1/contacts/{encodedPhone}/contact.vcf");
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
                var response = await _httpClient.PostAsJsonAsync("api/v1/contacts", contact);
                response.EnsureSuccessStatusCode();
                return new ApiResponse<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding contact");
                return new ApiResponse<bool> { Success = false, Message = $"Error adding contact: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> UpdateContactAsync(EmployeeContact contact)
        {
            try
            {
                // Construct the correct URL for the PUT request
                var url = "/api/v1/contacts";  // No need to include contact in the URL path

                // Send the PUT request with the contact data as JSON in the body
                var response = await _httpClient.PutAsJsonAsync(url, contact);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // If successful, return a successful ApiResponse
                return new ApiResponse<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the request
                _logger.LogError(ex, "Error updating contact");

                // Return a failure ApiResponse with the error message
                return new ApiResponse<bool> { Success = false, Message = $"Error updating contact: {ex.Message}" };
            }
        }

        public static string NormalizePhoneNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // If there is a space, return only the part after the space
            if (input.Contains(' '))
            {
                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                    input = parts[1];
                else
                    input = parts[0];
            }

            // Remove all non-digit characters
            var cleaned = new string(input.Where(char.IsDigit).ToArray());

            return cleaned;
        }
    }
} 