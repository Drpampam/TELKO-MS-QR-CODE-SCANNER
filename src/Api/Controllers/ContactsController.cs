using Application.DTOs;
using Application.Features.Constants;
using Application.Interfaces.Application;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ILogger<ApplicationService> _logger;
        private readonly IApplicationService _services;

        public ContactsController(ILogger<ApplicationService> logger, IApplicationService services)
        {
            _logger = logger;
            _services = services;
        }

        /// <summary>
        /// Imports employee contacts from a CSV or Excel file.
        /// </summary>
        /// <param name="file">The file to import (CSV or Excel format).</param>
        /// <returns>Returns import result status and message.</returns>
        /// <response code="200">Contacts imported successfully.</response>
        /// <response code="207">Partial success, some records failed.</response>
        /// <response code="400">Invalid file format or bad request.</response>
        /// <response code="500">Server error while processing file.</response>
        [HttpPost("import")]
        [SwaggerOperation(Summary = "Imports employee contacts from a CSV or Excel file")]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status207MultiStatus)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImportContacts(IFormFile file)
        {
            var response = await _services.ImportContactsFromFileAsync(file);

            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            else if (response.ResponseCode == ResponseCodes.PARTIAL_SUCCESS) { return StatusCode(207, response); }
            else if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }

        /// <summary>
        /// Gets the .vcf (vCard) file for an employee's contact details.
        /// </summary>
        /// <param name="phone">The phone number of the employee.</param>
        /// <returns>Returns .vcf file if found.</returns>
        /// <response code="200">vCard generated successfully.</response>
        /// <response code="400">Invalid phone number or contact not found.</response>
        /// <response code="500">Server error while generating vCard.</response>
        [HttpGet("{phone}/contact.vcf")]
        [SwaggerOperation(Summary = "Gets the .vcf of an employee contact details")]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContactCard([FromRoute] string phone)
        {
            var response = await _services.GetContactVcf(phone);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return File(response.Data, "text/vcard", $"{phone}.vcf"); }
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }

        /// <summary>
        /// Adds a new employee contact.
        /// </summary>
        /// <param name="newContact">Employee contact details to add.</param>
        /// <returns>Returns the newly added contact details.</returns>
        /// <response code="200">Contact added successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="500">Server error while adding contact.</response>
        [HttpPost]
        [SwaggerOperation(Summary = "Add new employee contact details")]
        [ProducesResponseType(typeof(BaseResponse<EmployeeContactDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddContact([FromBody] EmployeeContactDto newContact)
        {
            var response = await _services.AddContactAsync(newContact);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }

        /// <summary>
        /// Updates an existing employee contact.
        /// </summary>
        /// <param name="newContact">Employee contact details to update.</param>
        /// <returns>Returns the updated contact details.</returns>
        /// <response code="200">Contact updated successfully.</response>
        /// <response code="400">Invalid input data or contact not found.</response>
        /// <response code="500">Server error while updating contact.</response>
        [HttpPut]
        [SwaggerOperation(Summary = "Update employee contact details")]
        [ProducesResponseType(typeof(BaseResponse<EmployeeContactDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateContact([FromBody] EmployeeContactDto newContact)
        {
            var response = await _services.UpdateContactAsync(newContact);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }

        /// <summary>
        /// Gets a single employee's contact details by phone number.
        /// </summary>
        /// <param name="phone">The phone number of the employee.</param>
        /// <returns>Returns contact details if found.</returns>
        /// <response code="200">Contact found successfully.</response>
        /// <response code="400">Invalid phone number or contact not found.</response>
        /// <response code="500">Server error while retrieving contact.</response>
        [HttpGet("{phone}/contact-details")]
        [SwaggerOperation(Summary = "Gets single employee contact detail by phone")]
        [ProducesResponseType(typeof(BaseResponse<EmployeeContactDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContact([FromRoute] string phone)
        {
            var response = await _services.GetContactDetailsByPhone(phone);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }

        /// <summary>
        /// Gets all employee contact details.
        /// </summary>
        /// <param name="filter">Optional filter for search, pagination, etc.</param>
        /// <returns>Returns list of employee contacts.</returns>
        /// <response code="200">Contacts retrieved successfully.</response>
        /// <response code="400">Invalid filter criteria.</response>
        /// <response code="500">Server error while retrieving contacts.</response>
        [HttpPost("all-contacts")]
        [SwaggerOperation(Summary = "Gets all employee contact details")]
        [ProducesResponseType(typeof(BaseResponse<EmployeeContactReponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllContact(GetAllContacts filter = null!)
        {
            var response = await _services.GetAllContacts(filter);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }
    }
}
