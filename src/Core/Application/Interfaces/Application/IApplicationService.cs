using Application.DTOs;
using Application.Features.Pagination;
using Application.Responses;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Application
{
    public interface IApplicationService
    {
        Task<BaseResponse<byte[]>> GetContactVcf(string phone);
        Task<BaseResponse<byte[]>> GetContactQrCode(string phone);
        Task<BaseResponse<EmployeeContact>> AddContactAsync(EmployeeContactDto req);
        Task<BaseResponse<EmployeeContactDto>> GetContactDetailsByPhone(string phone);
        Task<BaseResponse<EmployeeContact>> UpdateContactAsync(EmployeeContactDto req);
        Task<BaseResponse<ImportResult>> ImportContactsFromFileAsync(IFormFile file);
        Task<BaseResponse<PaginationResult<EmployeeContactReponseDto>>> GetAllContacts(GetAllContacts filter = null!);
    }
}