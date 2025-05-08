using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTOs;
using WebUI.Services;

namespace WebUI.Pages.Contacts
{
    public class DetailsModel : PageModel
    {
        private readonly IEmployeeContactService _contactService;

        // Inject IEmployeeContactService instead of the concrete class
        public DetailsModel(IEmployeeContactService contactService)
        {
            _contactService = contactService;
        }

        public EmployeeContact? Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return NotFound();
            }

            var response = await _contactService.GetContactByPhoneAsync(phone);
            if (response.Data == null)
            {
                return NotFound();
            }

            Contact = response.Data;
            return Page();
        }
    }
}
