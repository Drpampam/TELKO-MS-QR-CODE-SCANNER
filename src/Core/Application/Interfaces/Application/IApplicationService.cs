using Domain.Entities;

namespace Application.Interfaces.Application
{
    public interface IApplicationService
    {
        byte[] GetContactQrCode(EmployeeContact contact);
        EmployeeContact GetMockContact(string name);
        byte[] GetContactVcf(EmployeeContact contact);
    }
}
