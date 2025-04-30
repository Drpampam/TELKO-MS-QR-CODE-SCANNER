using Application.Interfaces.Application;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using QRCoder;
using System.Text;

namespace Application.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IHelper _helper;
        private readonly ILogger<ApplicationService> _logger;
        public ApplicationService(IHelper helper, ILogger<ApplicationService> logger)
        {
            _helper = helper;
            _logger = logger;
        }

        public EmployeeContact GetMockContact(string name)
        {
            return new EmployeeContact
            {
                FullName = "John Omotoye",
                Phone = "+2348105633111",
                Email = "omotoyejohn2@gmail.com",
                Title = "Senior Backend Engineer",
                Company = "Your Company",
                LinkedIn = "https://www.linkedin.com/in/john-omotoye"
            };
        }

        public byte[] GetContactQrCode(EmployeeContact contact)
        {
            var vcard = _helper.ToVCard(contact);
            return _helper.GenerateQrCode(vcard);
        }

        public byte[] GetContactVcf(EmployeeContact contact)
        {
            var vcard = _helper.ToVCard(contact);
            return Encoding.UTF8.GetBytes(vcard);
        }
    }
}
