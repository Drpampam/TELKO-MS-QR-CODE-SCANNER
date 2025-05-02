using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Models;
using WebUI.Services;

namespace WebUI.Pages.Contacts
{
    public class CreateModel : PageModel
    {
        private readonly EmployeeContactService _contactService;

        public CreateModel(EmployeeContactService contactService)
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

            var success = await _contactService.AddContactAsync(Contact);
            if (success)
            {
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("", "Failed to create contact. Please try again.");
            return Page();
        }
    }
} 