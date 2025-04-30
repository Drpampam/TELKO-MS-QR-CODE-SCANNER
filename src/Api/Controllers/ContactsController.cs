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

        [HttpGet("{phone}/contact.vcf")]
        [SwaggerOperation(Summary = "gets the .vcf of an employee contact details")]
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

        [HttpPut]
        [SwaggerOperation(Summary = "update employee contact details")]
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

        [HttpGet("{phone}/contact-details")]    
        [SwaggerOperation(Summary = "single employee contact detail")]
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

        [HttpGet("all-contacts")]
        [SwaggerOperation(Summary = "all employee contact details")]
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
