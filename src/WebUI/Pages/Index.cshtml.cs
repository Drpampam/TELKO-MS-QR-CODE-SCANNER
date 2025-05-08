using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTOs;
using WebUI.Services;

namespace WebUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IEmployeeContactService _employeeContactService;

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

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _employeeContactService.GetContactsAsync(Phone, StartDate, EndDate);
                if (response.Success && response.Data != null)
                {
                    Contacts = response.Data.Items;
                }
                else
                {
                    ModelState.AddModelError("", response.Message ?? "Error getting contacts");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error getting contacts: {ex.Message}");
            }

            return Page();
        }
    }
}
