using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;

namespace WebUI.Pages.Contacts
{
    public class QRCodeModel : PageModel
    {
        private readonly EmployeeContactService _contactService;

        public QRCodeModel(EmployeeContactService contactService)
        {
            _contactService = contactService;
        }

        public byte[]? QRCodeBytes { get; private set; }
        public string? ErrorMessage { get; private set; }

        public async Task<IActionResult> OnGetAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                ErrorMessage = "Phone number is required";
                return Page();
            }

            try
            {
                var response = await _contactService.GetQRCodeAsync(phone);
                if (!response.IsSuccess || response.Data == null)
                {
                    ErrorMessage = response.Message ?? "Failed to generate QR code";
                    return Page();
                }

                QRCodeBytes = response.Data;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error generating QR code: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return BadRequest("Phone number is required");
            }

            try
            {
                var response = await _contactService.GetQRCodeAsync(phone);
                if (!response.IsSuccess || response.Data == null)
                {
                    return BadRequest(response.Message ?? "Failed to generate QR code");
                }

                return File(response.Data, "image/png", $"qrcode-{phone}.png");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error generating QR code: {ex.Message}");
            }
        }
    }
} 