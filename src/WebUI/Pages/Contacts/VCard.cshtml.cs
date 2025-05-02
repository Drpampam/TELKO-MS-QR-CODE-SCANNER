using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Services;

namespace WebUI.Pages.Contacts
{
    public class VCardModel : PageModel
    {
        private readonly EmployeeContactService _contactService;

        public VCardModel(EmployeeContactService contactService)
        {
            _contactService = contactService;
        }

        public async Task<IActionResult> OnGetAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return NotFound();
            }

            var vcardBytes = await _contactService.GetVCardAsync(phone);
            if (vcardBytes == null)
            {
                return NotFound();
            }

            return File(vcardBytes, "text/vcard", $"{phone}.vcf");
        }
    }
} 