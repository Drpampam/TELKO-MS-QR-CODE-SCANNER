using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTOs;
using WebUI.Services;

namespace WebUI.Pages.Contacts
{
    public class CreateModel : PageModel
    {
        private readonly IEmployeeContactService _contactService;

        // Inject IEmployeeContactService instead of concrete class EmployeeContactService
        public CreateModel(IEmployeeContactService contactService)
        {
            _contactService = contactService;
        }

        [BindProperty]
        public EmployeeContact? Contact { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Contact == null)
            {
                return Page();
            }

            var response = await _contactService.AddContactAsync(Contact);
            if (response.Success)
            {
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("", response.Message ?? "Failed to create contact. Please try again.");
            return Page();
        }
    }
}
