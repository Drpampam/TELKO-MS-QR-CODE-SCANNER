using Domain.Entities;

namespace Application.Interfaces.Application
{
    public interface IHelper
    {
        byte[] GenerateQrCode(string payload);
        string GenerateVCard(EmployeeContact contact);
        string ToVCard(EmployeeContact contact);
    }
}
