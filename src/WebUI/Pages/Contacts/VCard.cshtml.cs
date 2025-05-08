using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;

namespace WebUI.Pages.Contacts
{
    public class VCardModel : PageModel
    {
        private readonly IEmployeeContactService _contactService;

        public VCardModel(IEmployeeContactService contactService)
        {
            _contactService = contactService;
        }

        public async Task<IActionResult> OnGetAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return NotFound();
            }

            var response = await _contactService.GetVCardAsync(phone);
            if ( response.Data == null)
            {
                return NotFound();
            }

            return File(response.Data, "text/vcard", $"{phone}.vcf");
        }
    }
} 