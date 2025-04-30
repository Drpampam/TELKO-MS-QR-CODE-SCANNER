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
    public class QRCodeController : ControllerBase
    {
        private readonly ILogger<ApplicationService> _logger;
        private readonly IApplicationService _services;

        public QRCodeController(ILogger<ApplicationService> logger, IApplicationService services)
        {
            _logger = logger;
            _services = services;
        }

        [HttpPost("{phone}/qrcode")]     
        [SwaggerOperation(Summary = "gets the QR code of an employee contact details")]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContactQrCode([FromRoute] string phone)
        {
            var response = await _services.GetContactQrCode(phone);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return File(response.Data, "image/png"); }
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }
    }
}
