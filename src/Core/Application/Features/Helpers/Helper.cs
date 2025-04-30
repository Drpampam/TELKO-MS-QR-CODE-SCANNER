using Application.DTOs;
using Application.Interfaces.Application;
using Microsoft.Extensions.Logging;
using QRCoder;
using System.Text;

namespace Application.Features.Helpers
{
    public class Helper : IHelper
    {
        private readonly ILogger<Helper> _logger;

        public Helper(ILogger<Helper> logger)
        {
            _logger = logger;
        }

        public byte[] GenerateQrCode(string payload)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        public string GenerateVCard(EmployeeContactDto contact)
        {
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCARD");
            sb.AppendLine("VERSION:3.0");
            sb.AppendLine($"N:{contact.FullName.Split(' ')[1]};{contact.FullName.Split(' ')[0]};;;");
            sb.AppendLine($"FN:{contact.FullName}");
            sb.AppendLine($"ORG:{contact.Company}");
            sb.AppendLine($"TITLE:{contact.Title}");
            sb.AppendLine($"TEL;TYPE=CELL:{contact.Phone}");
            sb.AppendLine($"EMAIL:{contact.Email}");
            sb.AppendLine($"URL:{contact.LinkedIn}");
            sb.AppendLine("END:VCARD");
            return sb.ToString();
        }

        public string ToVCard(EmployeeContactDto contact)
        {
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCARD");
            sb.AppendLine("VERSION:3.0");

            var nameParts = contact.FullName.Split(' ');
            var firstName = nameParts.Length > 0 ? nameParts[0] : "";
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";
            sb.AppendLine($"N:{lastName};{firstName};;;");
            sb.AppendLine($"FN:{contact.FullName}");
            sb.AppendLine($"ORG:{contact.Company}");
            sb.AppendLine($"TITLE:{contact.Title}");
            sb.AppendLine($"TEL;TYPE=CELL:{contact.Phone}");
            sb.AppendLine($"EMAIL:{contact.Email}");
            sb.AppendLine($"URL:{contact.LinkedIn}");
            sb.AppendLine("END:VCARD");
            return sb.ToString();
        }
    }
}
