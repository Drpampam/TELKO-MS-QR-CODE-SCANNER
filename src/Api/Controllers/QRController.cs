using Application.Interfaces.Application;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRController : ControllerBase
    {
        private readonly ILogger<ApplicationService> _logger;
        private readonly IApplicationService _services;

        public QRController(ILogger<ApplicationService> logger, IApplicationService services)
        {
            _logger = logger;
            _services = services;
        }

        [HttpGet("{name}/contact.vcf")]
        public IActionResult GetContactCard([FromRoute] string name)
        {
            var contact = _services.GetMockContact(name);
            var vcfBytes = _services.GetContactVcf(contact);
            return File(vcfBytes, "text/vcard", $"{name}.vcf");
        }

        [HttpGet("{name}/qrcode")]
        public IActionResult GetContactQrCode([FromRoute] string name)
        {
            var contact = _services.GetMockContact(name);
            var qrBytes = _services.GetContactQrCode(contact);
            return File(qrBytes, "image/png");
        }
    }
}
