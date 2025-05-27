using System.ComponentModel.DataAnnotations;
using Application.Features.ExtentionHelpers;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static QRCoder.PayloadGenerator;

namespace Application.DTOs
{
    public class EmployeeContactDto
    {
        public string FullName { get; set; } = string.Empty;
        public string PersonalPhone { get; set; } = string.Empty;
        public string WorkPhone { get; set; } = null!; public string Email { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string LinkedIn { get; set; } = string.Empty;

        public void ConvertToDto(EmployeeContact employee)
        {
            if (employee == null)
                employee = new EmployeeContact();

            FullName = employee.FullName;
            PersonalPhone = employee.PersonalPhone;
            WorkPhone = employee.WorkPhone;
            Email = employee.Email;
            Title = employee.Title;
            Company = employee.Company;
            LinkedIn = employee.LinkedIn;
        }

        public void ConvertFromDto(EmployeeContact employee)
        {
            if (employee == null)
                employee = new EmployeeContact();

            employee.FullName = FullName;
            employee.WorkPhone = WorkPhone;
            employee.PersonalPhone = PersonalPhone;
            employee.Email = Email;
            employee.Title = Title;
            employee.Company = Company;
            employee.LinkedIn = LinkedIn;
        }
    }

    public record EmployeeContactReponseDto
    {
        public string FullName { get; set; } = string.Empty;
        public string PersonalPhone { get; set; } = string.Empty;
        public string WorkPhone { get; set; } = null!; public string Email { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string LinkedIn { get; set; } = string.Empty;

        public void ConvertToDto(EmployeeContact employee)
        {
            if (employee == null)
                employee = new EmployeeContact();


            FullName = employee.FullName;
            PersonalPhone = employee.PersonalPhone;
            WorkPhone = employee.WorkPhone;
            Email = employee.Email;
            JobTitle = employee.Title;
            Company = employee.Company;
            LinkedIn = employee.LinkedIn;
        }
    }

    public record GetAllContacts
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Phone { get; set; }

        public void MapToFilterDateConvert(FilterDateConvert dto)
        {
            dto.StartDate = HelperExtensions.ConvertToDateTime(StartDate!);
            dto.EndDate = HelperExtensions.ConvertToDateTime(EndDate!);
        }

        public record FilterDateConvert
        {
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
