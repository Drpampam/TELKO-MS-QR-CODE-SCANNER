using Application.DTOs;
using Application.Features.Constants;
using Application.Features.Pagination;
using Application.Features.Validations;
using Application.Interfaces;
using Application.Interfaces.Application;
using Application.Responses;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using static Application.DTOs.GetAllContacts;

namespace Application.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IHelper _helper;
        private readonly ILogger<ApplicationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAsyncRepository<EmployeeContact> _repository;
        private readonly EmployeeContactFormValidator _validator;
        public ApplicationService(IHelper helper,
            ILogger<ApplicationService> logger,
            IAsyncRepository<EmployeeContact> repository,
            IUnitOfWork unitOfWork, EmployeeContactFormValidator validationRules)
        {
            _helper = helper;
            _logger = logger;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _validator = validationRules;
        }

        public async Task<BaseResponse<EmployeeContact>> AddContactAsync(EmployeeContactDto req)
        {
            try
            {
                var validationResult = _validator.Validate(req);
                if (!validationResult.IsValid)
                {
                    var validationErrors = validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    }).ToList();

                    return new BaseResponse<EmployeeContact>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = JsonSerializer.Serialize(validationErrors)
                                + ": Validation failed"
                    };
                }

                EmployeeContact employeeContact = new EmployeeContact();
                req.ConvertFromDto(employeeContact);
                if (string.IsNullOrEmpty(employeeContact.WorkPhone))
                {
                    return new BaseResponse<EmployeeContact>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Phone number is required.",
                    };
                }

                var existingContact = await GetContactDetailsByPhone(employeeContact.WorkPhone);

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
                if (string.IsNullOrEmpty(req.WorkPhone))
                {
                    return new BaseResponse<EmployeeContact>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Phone number is required.",
                    };
                }

                // Check if the contact exists
                var existingContactResponse = await _repository.SingleOrDefaultAsync(x => x.WorkPhone == req.WorkPhone);
                if (existingContactResponse == null)
                {
                    return new BaseResponse<EmployeeContact>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "Contact not found.",
                    };
                }

                req.ConvertFromDto(existingContactResponse);

                // Update contact
                var result = await _repository.UpdateAsync(existingContactResponse);
                await _unitOfWork.CommitChangesAsync();

                return new BaseResponse<EmployeeContact>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Contact updated successfully.",
                    Data = existingContactResponse
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
                var employeeContact = await _repository
                    .SingleOrDefaultAsync(b => b.WorkPhone != null && b.WorkPhone.Contains(phone));

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

        public async Task<BaseResponse<PaginationResult<EmployeeContactReponseDto>>> GetAllContactsV2(GetAllContacts filter = null!)
        {
            try
            {
                var logRequest = JsonSerializer.Serialize(filter);
                _logger.LogInformation(logRequest);

                var dateFilter = new FilterDateConvert();
                filter?.MapToFilterDateConvert(dateFilter);

                var requests = await _repository.WhereQueryable(x => x != null);

                if (!string.IsNullOrEmpty(filter?.Phone)) requests = requests.Where(x => x.WorkPhone == filter.Phone);

                if (filter?.StartDate != null && filter?.EndDate != null)
                {
                    _logger.LogInformation($"Filtering between {dateFilter.StartDate} and {dateFilter.EndDate}");
                    requests = requests.Where(x =>
                        x.CreatedAt >= dateFilter.StartDate && x.CreatedAt <= dateFilter.EndDate);
                }          

                var result = await requests.OrderByDescending(x => x.Id)
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

        public async Task<BaseResponse<PaginationResult<EmployeeContactReponseDto>>> GetAllContacts(GetAllContacts? filter = null)
        {
            try
            {
                filter ??= new GetAllContacts(); // Ensure it's not null

                var logRequest = JsonSerializer.Serialize(filter);
                _logger.LogInformation(logRequest);

                var dateFilter = new FilterDateConvert();
                filter.MapToFilterDateConvert(dateFilter);

                var requests = await _repository.WhereQueryable(x => x != null);

                if (!string.IsNullOrEmpty(filter.Phone))
                    requests = requests.Where(x => x.WorkPhone == filter.Phone);

                if (filter.StartDate != null && filter.EndDate != null)
                {
                    _logger.LogInformation($"Filtering between {dateFilter.StartDate} and {dateFilter.EndDate}");
                    requests = requests.Where(x =>
                        x.CreatedAt >= dateFilter.StartDate && x.CreatedAt <= dateFilter.EndDate);
                }

                var result = await requests.OrderByDescending(x => x.Id)
                    .PaginateAsync(filter.PageNumber, filter.PageSize);

                var logResponse = JsonSerializer.Serialize(result);
                _logger.LogInformation(logResponse);

                if (result.Items == null || !result.Items.Any())
                {
                    return new BaseResponse<PaginationResult<EmployeeContactReponseDto>>
                    {
                        Message = "No record found",
                        ResponseCode = ResponseCodes.NOT_FOUND
                    };
                }

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
                    $"Retrieved page {filter.PageNumber} of requests successfully",
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

        /// <summary>
        /// Imports employee contacts from an Excel or CSV file
        /// </summary>
        /// <param name="fileStream">The uploaded file stream</param>
        /// <param name="fileName">The original file name with extension</param>
        /// <returns>A response with the import results</returns>
        public async Task<BaseResponse<ImportResult>> ImportContactsFromFileAsync(IFormFile file)
        {
            try
            {
                var fileName = file.FileName;
                _logger.LogInformation($"Starting import of contacts from file: {fileName}");

                // Validate file
                if (file == null || file.Length == 0)
                {
                    return new BaseResponse<ImportResult>
                    {
                        ResponseCode = ResponseCodes.INVALID_REQUEST,
                        Message = "Please upload a file"
                    };
                }

                // Check file extension
                string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (fileExtension != ".csv" && fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    return new BaseResponse<ImportResult>
                    {
                        ResponseCode = ResponseCodes.INVALID_REQUEST,
                        Message = "Only CSV and Excel files are supported"
                    };
                }

                List<EmployeeContactDto> contacts;

                // Read and parse file
                using (var fileStream = file.OpenReadStream())
                {
                    if (fileExtension == ".csv")
                    {
                        contacts = await ParseCsvFile(fileStream);
                    }
                    else // Excel formats
                    {
                        contacts = await ParseExcelFile(fileStream, fileExtension);
                    }
                }

                // If no contacts were parsed, return an error
                if (contacts == null || !contacts.Any())
                {
                    return new BaseResponse<ImportResult>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "No valid contacts found in the file."
                    };
                }

                _logger.LogInformation($"Parsed {contacts.Count} contacts from file");

                // Process each contact
                int successCount = 0;
                int failureCount = 0;
                List<string> errors = new List<string>();

                foreach (var contact in contacts)
                {
                    // Validate the contact
                    var validationResult = _validator.Validate(contact);
                    if (!validationResult.IsValid)
                    {
                        string contactIdentifier = !string.IsNullOrEmpty(contact.FullName)
                            ? contact.FullName
                            : (!string.IsNullOrEmpty(contact.WorkPhone) ? contact.WorkPhone : "Unknown");

                        string errorDetails = $"Validation failed for contact '{contactIdentifier}': " +
                                              string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));

                        errors.Add(errorDetails);
                        failureCount++;
                        continue;
                    }

                    // Check if contact already exists
                    if (!string.IsNullOrEmpty(contact.WorkPhone))
                    {
                        var existingContact = await GetContactDetailsByPhone(contact.WorkPhone);
                        if (existingContact.ResponseCode == ResponseCodes.SUCCESS)
                        {
                            errors.Add($"Contact with phone number '{contact.WorkPhone}' already exists");
                            failureCount++;
                            continue;
                        }
                    }

                    // Add the contact
                    EmployeeContact employeeContact = new EmployeeContact();
                    contact.ConvertFromDto(employeeContact);

                    await _repository.AddAsync(employeeContact);
                    successCount++;
                }

                // Commit all changes to database
                await _unitOfWork.CommitChangesAsync();

                // Prepare result
                var importResult = new ImportResult
                {
                    TotalProcessed = contacts.Count,
                    SuccessCount = successCount,
                    FailureCount = failureCount,
                    Errors = errors
                };

                return new BaseResponse<ImportResult>
                {
                    ResponseCode = successCount > 0 ? ResponseCodes.SUCCESS : ResponseCodes.DUPLICATE_RESOURCE,
                    Message = GetImportResultMessage(successCount, failureCount),
                    Data = importResult
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while importing contacts from file");
                return new BaseResponse<ImportResult>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while importing contacts: " + ex.Message
                };
            }
        }


        private async Task<List<EmployeeContactDto>> ParseCsvFile(Stream fileStream)
        {
            List<EmployeeContactDto> contacts = new List<EmployeeContactDto>();

            // Create a stream reader
            using (var reader = new StreamReader(fileStream))
            {
                // Read the CSV data
                string csvData = await reader.ReadToEndAsync();

                // Parse the CSV using Papaparse-like approach
                var lines = csvData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length < 2) // At least header + one data row
                {
                    return contacts;
                }

                // Parse header row
                var headerRow = lines[0].Split(',');
                var headerIndexMap = new Dictionary<string, int>();

                // Map header names to column indices
                for (int i = 0; i < headerRow.Length; i++)
                {
                    string headerName = headerRow[i].Trim().ToLowerInvariant();
                    headerIndexMap[headerName] = i;
                }

                // Process data rows
                for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
                {
                    var dataRow = lines[lineIndex].Split(',');
                    if (dataRow.Length < headerRow.Length)
                    {
                        continue; // Skip incomplete rows
                    }

                    var contact = new EmployeeContactDto();

                    // Map CSV fields to DTO properties
                    if (headerIndexMap.TryGetValue("fullname", out int fullNameIndex))
                        contact.FullName = CleanField(dataRow[fullNameIndex]);

                    if (headerIndexMap.TryGetValue("phone", out int phoneIndex))
                        contact.WorkPhone = CleanField(dataRow[phoneIndex]);

                    if (headerIndexMap.TryGetValue("email", out int emailIndex))
                        contact.Email = CleanField(dataRow[emailIndex]);

                    if (headerIndexMap.TryGetValue("title", out int titleIndex))
                        contact.Title = CleanField(dataRow[titleIndex]);

                    if (headerIndexMap.TryGetValue("company", out int companyIndex))
                        contact.Company = CleanField(dataRow[companyIndex]);

                    if (headerIndexMap.TryGetValue("linkedin", out int linkedinIndex))
                        contact.LinkedIn = CleanField(dataRow[linkedinIndex]);

                    contacts.Add(contact);
                }
            }

            return contacts;
        }

        private async Task<List<EmployeeContactDto>> ParseExcelFile(Stream fileStream, string fileExtension)
        {
            List<EmployeeContactDto> contacts = new List<EmployeeContactDto>();

            // Use a memory stream since we need to reset the position
            using (var ms = new MemoryStream())
            {
                await fileStream.CopyToAsync(ms);
                ms.Position = 0;

                using (var workbook = new ClosedXML.Excel.XLWorkbook(ms))
                {
                    // Assume data is in the first worksheet
                    var worksheet = workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return contacts;
                    }

                    // Get the range of cells with data
                    var dataRange = worksheet.RangeUsed();
                    if (dataRange == null)
                    {
                        return contacts;
                    }

                    // Get the headers from the first row
                    var headerRow = dataRange.FirstRow();
                    var headerIndexMap = new Dictionary<string, int>();

                    // Map header names to column indices
                    int colIndex = 0;
                    foreach (var cell in headerRow.Cells())
                    {
                        string headerName = cell.Value.ToString().Trim().ToLowerInvariant();
                        headerIndexMap[headerName] = colIndex;
                        colIndex++;
                    }

                    // Process data rows (skip header row)
                    for (int rowIndex = 2; rowIndex <= dataRange.LastRow().RowNumber(); rowIndex++)
                    {
                        var row = worksheet.Row(rowIndex);

                        var contact = new EmployeeContactDto();

                        // Map Excel fields to DTO properties
                        if (headerIndexMap.TryGetValue("fullname", out int fullNameIndex))
                            contact.FullName = CleanField(row.Cell(fullNameIndex + 1).Value.ToString());

                        if (headerIndexMap.TryGetValue("phone", out int phoneIndex))
                            contact.WorkPhone = CleanField(row.Cell(phoneIndex + 1).Value.ToString());

                        if (headerIndexMap.TryGetValue("email", out int emailIndex))
                            contact.Email = CleanField(row.Cell(emailIndex + 1).Value.ToString());

                        if (headerIndexMap.TryGetValue("title", out int titleIndex))
                            contact.Title = CleanField(row.Cell(titleIndex + 1).Value.ToString());

                        if (headerIndexMap.TryGetValue("company", out int companyIndex))
                            contact.Company = CleanField(row.Cell(companyIndex + 1).Value.ToString());

                        if (headerIndexMap.TryGetValue("linkedin", out int linkedinIndex))
                            contact.LinkedIn = CleanField(row.Cell(linkedinIndex + 1).Value.ToString());

                        contacts.Add(contact);
                    }
                }
            }

            return contacts;
        }

        private string CleanField(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Remove quotes and trim whitespace
            value = value.Trim().Trim('"', '\'');

            // Check if it's the literal "string" placeholder
            if (value.Trim().ToLowerInvariant() == "string")
                return string.Empty;

            return value;
        }

        private string GetImportResultMessage(int successCount, int failureCount)
        {
            if (successCount > 0 && failureCount == 0)
                return $"Successfully imported {successCount} contacts.";
            else if (successCount > 0 && failureCount > 0)
                return $"Imported {successCount} contacts successfully with {failureCount} failures.";
            else
                return $"Failed to import any contacts. {failureCount} contacts had validation errors.";
        }      
    }
}