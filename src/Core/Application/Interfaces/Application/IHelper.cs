using Application.DTOs;

namespace Application.Interfaces.Application
{
    public interface IHelper
    {
        byte[] GenerateQrCode(string payload);
        string GenerateVCard(EmployeeContactDto contact);
        string ToVCard(EmployeeContactDto contact);
    }
}
