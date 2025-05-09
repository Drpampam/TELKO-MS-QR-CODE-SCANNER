using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPOI.XSSF.UserModel; // For .xlsx
using NPOI.HSSF.UserModel; // For .xls
using System.ComponentModel;
using System.Globalization;
using WebUI.DTOs;
using WebUI.DTOs.ContactRequestDtos;
using WebUI.Services;
using System.IO;
using NPOI.SS.UserModel;

namespace WebUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IEmployeeContactService _employeeContactService;

        private static readonly string[] AllowedExtensions = { ".csv", ".xlsx", ".xls" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private const int BatchSize = 100;

        public IndexModel(IEmployeeContactService employeeContactService)
        {
            _employeeContactService = employeeContactService;
        }

        public List<EmployeeContact> Contacts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Phone { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? EndDate { get; set; }

        [BindProperty]
        public IFormFile File { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var filter = new GetAllContacts
                {
                    Phone = Phone,
                    StartDate = StartDate,
                    EndDate = EndDate
                };

                var response = await _employeeContactService.GetContactsAsync(filter);

                if (response.Success && response.Data?.Items != null)
                {
                    Contacts = response.Data.Items;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message ?? "Error retrieving contacts.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ValidateFile(File))
                return await OnGetAsync();

            try
            {
                var newContacts = await ParseUploadedFileAsync(File);

                var validationErrors = ValidateContacts(newContacts);
                if (validationErrors.Any())
                {
                    foreach (var error in validationErrors)
                        ModelState.AddModelError("File", error);

                    return await OnGetAsync();
                }

                await SaveContactsInBatchesAsync(newContacts);

                TempData["UploadSuccess"] = "Contacts uploaded successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("File", $"Error processing the file: {ex.Message}");
                return await OnGetAsync();
            }
        }

        private bool ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please upload a valid file.");
                return false;
            }

            if (file.Length > MaxFileSize)
            {
                ModelState.AddModelError("File", "File size must be less than 5MB.");
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("File", "Only CSV or Excel files are allowed.");
                return false;
            }

            return true;
        }

        private async Task<List<EmployeeContact>> ParseUploadedFileAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            return extension == ".csv"
                ? ParseCsv(stream)
                : ParseExcel(stream);
        }

        private List<EmployeeContact> ParseCsv(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<EmployeeContact>().ToList();
        }

        private List<EmployeeContact> ParseExcel(Stream stream)
        {
            var contacts = new List<EmployeeContact>();

            // Check if it's an XLSX or XLS file
            IWorkbook workbook = null;
            try
            {
                workbook = new XSSFWorkbook(stream); // For .xlsx
            }
            catch
            {
                stream.Position = 0; // Reset stream position to try with HSSF (xls)
                workbook = new HSSFWorkbook(stream); // For .xls
            }

            var sheet = workbook.GetSheetAt(0); // Get the first sheet
            var rowCount = sheet.LastRowNum;

            for (int row = 1; row <= rowCount; row++) // Skip header row
            {
                var currentRow = sheet.GetRow(row);

                contacts.Add(new EmployeeContact
                {
                    FullName = currentRow.GetCell(0).ToString(),
                    Phone = currentRow.GetCell(1).ToString(),
                    Email = currentRow.GetCell(2).ToString(),
                    Title = currentRow.GetCell(3).ToString(),
                    Company = currentRow.GetCell(4).ToString()
                });
            }

            return contacts;
        }

        private List<string> ValidateContacts(IEnumerable<EmployeeContact> contacts)
        {
            var errors = new List<string>();

            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var phoneRegex = new System.Text.RegularExpressions.Regex(@"^(?:\+234|0)(7[0-9]|8[0-9]|9[0-9])[0-9]{7}$");

            foreach (var contact in contacts)
            {
                // Nigerian Phone Validation
                if (string.IsNullOrWhiteSpace(contact.Phone) || !phoneRegex.IsMatch(contact.Phone))
                {
                    errors.Add($"Invalid phone number for {contact.FullName}. " +
                               "Phone must be Nigerian format: +2348012345678 or 08012345678.");
                }

                // Basic Email Validation
                if (string.IsNullOrWhiteSpace(contact.Email) || !emailRegex.IsMatch(contact.Email))
                {
                    errors.Add($"Invalid email address for {contact.FullName}. " +
                               "Email must be valid like: name@example.com.");
                }
            }

            return errors;
        }

        private async Task SaveContactsInBatchesAsync(List<EmployeeContact> contacts)
        {
            for (int i = 0; i < contacts.Count; i += BatchSize)
            {
                var batch = contacts.Skip(i).Take(BatchSize);

                foreach (var contact in batch)
                {
                    var response = await _employeeContactService.CreateContactAsync(contact);

                    if (!response.Success)
                    {
                        throw new InvalidOperationException($"Failed to save contact {contact.FullName}: {response.Message}");
                    }
                }
            }
        }
    }
}
