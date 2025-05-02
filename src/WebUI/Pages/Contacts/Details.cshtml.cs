using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Models;
using WebUI.Services;

namespace WebUI.Pages.Contacts
{
    public class DetailsModel : PageModel
    {
        private readonly EmployeeContactService _contactService;

        public DetailsModel(EmployeeContactService contactService)
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
            if (!response.IsSuccess || response.Data == null)
            {
                return NotFound();
            }

            Contact = response.Data;
            return Page();
        }
    }
} 