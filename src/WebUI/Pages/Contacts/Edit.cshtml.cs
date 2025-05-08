using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTOs;
using WebUI.DTOs.ContactRequestDtos;
using WebUI.Services;

namespace WebUI.Pages.Contacts
{
    public class EditModel : PageModel
    {
        private readonly IEmployeeContactService _contactService;

        [BindProperty]
        public EmployeeContact Contact { get; set; }

        public EditModel(IEmployeeContactService contactService)
        {
            _contactService = contactService;
        }

        public async Task<IActionResult> OnGetAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return NotFound();
            }

            var contact = await _contactService.GetContactByPhoneAsync(phone);

            if (contact == null)
            {
                return NotFound();
            }

            Contact = contact.Data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Update the contact
            var result = await _contactService.UpdateContactAsync(Contact);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Contact updated successfully!";
                return RedirectToPage("/Contacts/Details", new { phone = Contact.Phone });
            }
            else
            {
                // Handle error if the update fails
                TempData["ErrorMessage"] = "Error updating contact.";
                return Page();
            }
        }
    }

}
