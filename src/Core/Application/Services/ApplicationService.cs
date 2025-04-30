using Application.DTOs;
using Application.Features.Constants;
using Application.Features.Pagination;
using Application.Interfaces;
using Application.Interfaces.Application;
using Application.Responses;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Text;
using static Application.DTOs.GetAllContacts;
using System.Text.Json;

namespace Application.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IHelper _helper;
        private readonly ILogger<ApplicationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAsyncRepository<EmployeeContact> _repository;
        public ApplicationService(IHelper helper, ILogger<ApplicationService> logger, IAsyncRepository<EmployeeContact> repository, IUnitOfWork unitOfWork)
        {
            _helper = helper;
            _logger = logger;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<EmployeeContact>> AddContactAsync(EmployeeContactDto req)
        {
            try
            {
                EmployeeContact employeeContact = new EmployeeContact();
                req.ConvertFromDto(employeeContact);
                if (string.IsNullOrEmpty(employeeContact.Phone))
                {
                    return new BaseResponse<EmployeeContact>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Phone number is required.",
                    };
                }

                var existingContact = await GetContactDetailsByPhone(employeeContact.Phone);

                if (existingContact.ResponseCode == ResponseCodes.SUCCESS)
                {
                    return new BaseResponse<EmployeeContact>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Contact already exists.",
                    };
                }

                var result = await _repository.AddAsync(employeeContact);
                await _unitOfWork.CommitChangesAsync();

                return (new BaseResponse<EmployeeContact>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Contact added successfully.",
                    Data = employeeContact
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding contact details.");
                return new BaseResponse<EmployeeContact>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while adding contact details.",
                };
            }
        }

        public async Task<BaseResponse<EmployeeContact>> UpdateContactAsync(EmployeeContactDto req)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(req.Phone))
                {
                    return new BaseResponse<EmployeeContact>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Phone number is required.",
                    };
                }

                // Check if the contact exists
                var existingContactResponse = await GetContactDetailsByPhone(req.Phone);
                if (existingContactResponse.ResponseCode != ResponseCodes.SUCCESS)
                {
                    return new BaseResponse<EmployeeContact>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "Contact not found.",
                    };
                }

                var employeeContact = new EmployeeContact();
                req.ConvertFromDto(employeeContact);

                // Update contact
                var result = await _repository.UpdateAsync(employeeContact);
                await _unitOfWork.CommitChangesAsync();

                return new BaseResponse<EmployeeContact>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Contact updated successfully.",
                    Data = employeeContact
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating contact details.");
                return new BaseResponse<EmployeeContact>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while updating contact details.",
                };
            }
        }

        public async Task<BaseResponse<EmployeeContactDto>> GetContactDetailsByPhone(string phone)
        {
            try
            {
                var employeeContact = await _repository.SingleOrDefaultAsync(b => b.Phone == phone);
                if (employeeContact == null)
                {
                    return new BaseResponse<EmployeeContactDto>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "Contact not found.",
                        //Data = null
                    };
                }

                var employeeContactDto = new EmployeeContactDto();
                employeeContactDto.ConvertToDto(employeeContact);
                var employeeDetails = new BaseResponse<EmployeeContactDto>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Contact found.",
                    Data = employeeContactDto
                };

                return employeeDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contact details for phone: {Phone}", phone);
                return new BaseResponse<EmployeeContactDto>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while retrieving contact details.",
                    //Data = null
                };
            }
        }

        public async Task<BaseResponse<PaginationResult<EmployeeContactReponseDto>>> GetAllContacts(GetAllContacts filter = null!)
        {
            try
            {
                var logRequest = JsonSerializer.Serialize(filter);
                _logger.LogInformation(logRequest);

                var dateFilter = new FilterDateConvert();
                filter?.MapToFilterDateConvert(dateFilter);

                var requests = await _repository.WhereQueryable(x => x != null);

                if (!string.IsNullOrEmpty(filter?.Phone)) requests = requests.Where(x => x.Phone == filter.Phone);

                if (filter?.StartDate != null && filter?.EndDate != null)
                {
                    _logger.LogInformation($"Filtering between {dateFilter.StartDate} and {dateFilter.EndDate}");
                    requests = requests.Where(x =>
                        x.CreatedAt >= dateFilter.StartDate && x.CreatedAt <= dateFilter.EndDate);
                }
                else
                {
                    var todayStart = DateTime.Today;
                    var todayEnd = DateTime.Today.AddDays(1).AddTicks(-1); // End of today's date
                    _logger.LogInformation($"Filtering for today's date: {todayStart} to {todayEnd}");
                    requests = requests.Where(x => x.CreatedAt >= todayStart && x.CreatedAt <= todayEnd);
                }

                var result = await requests.OrderByDescending(x => x.CreatedAt)
                    .PaginateAsync(filter!.PageNumber, filter.PageSize);

                var logResponse = JsonSerializer.Serialize(result);
                _logger.LogInformation(logResponse);

                if (result.Items == null || !result.Items.Any())
                    return new BaseResponse<PaginationResult<EmployeeContactReponseDto>>
                    {
                        Message = "No record found",
                        ResponseCode = ResponseCodes.NOT_FOUND
                    };
                var files = new List<EmployeeContactReponseDto>();
                foreach (var request in result.Items)
                {
                    var requestDto = new EmployeeContactReponseDto();
                    requestDto.ConvertToDto(request);
                    files.Add(requestDto);
                }

                var pagedResponse = new PaginationResult<EmployeeContactReponseDto>(
                    files.AsQueryable(),
                    filter.PageNumber,
                    filter.PageSize,
                    result.TotalCount,
                    result.TotalPages);

                var logResult = JsonSerializer.Serialize(pagedResponse);
                _logger.LogInformation(logResult);

                return new BaseResponse<PaginationResult<EmployeeContactReponseDto>>(
               $"Retrieved page {filter?.PageNumber ?? 1} of requests successfully",
               pagedResponse,
               ResponseCodes.SUCCESS);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching file details.");
                return new BaseResponse<PaginationResult<EmployeeContactReponseDto>>
                {
                    Message = "An error occurred while fetching file details.",
                    ResponseCode = ResponseCodes.FAILURE
                };
            }
        }

        public async Task<BaseResponse<byte[]>> GetContactQrCode(string phone)
        {
            try
            {

                var contact = await GetContactDetailsByPhone(phone);
                var vcard = _helper.ToVCard(contact.Data);
                return new BaseResponse<byte[]>()
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Contact QR code generated successfully.",
                    Data = _helper.GenerateQrCode(vcard)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating Qrcode for contact details phone: {Phone}", phone);
                return new BaseResponse<byte[]>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while generating Qrcode for contact details.",
                    //Data = null
                };
            }
        }

        public async Task<BaseResponse<byte[]>> GetContactVcf(string phone)
        {
            try
            {

                var contact = await GetContactDetailsByPhone(phone);
                var vcard = _helper.ToVCard(contact.Data);
                return new BaseResponse<byte[]>()
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Contact QR code generated successfully.",
                    Data = Encoding.UTF8.GetBytes(vcard)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating Qrcode for contact details phone: {Phone}", phone);
                return new BaseResponse<byte[]>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while generating Qrcode for contact details.",
                    //Data = null
                };
            }
        }
    }
}