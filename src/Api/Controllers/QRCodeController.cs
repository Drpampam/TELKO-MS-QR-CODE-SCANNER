using Application.Features.Constants;
using Application.Interfaces.Application;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing employee contact QR codes.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class QRCodeController : ControllerBase
    {
        private readonly ILogger<ApplicationService> _logger;
        private readonly IApplicationService _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="QRCodeController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="services">Application service instance.</param>
        public QRCodeController(ILogger<ApplicationService> logger, IApplicationService services)
        {
            _logger = logger;
            _services = services;
        }

        /// <summary>
        /// Generates and retrieves the QR code image for an employee's contact details.
        /// </summary>
        /// <param name="phone">The phone number of the employee to generate the QR code for.</param>
        /// <returns>
        /// Returns a PNG image containing the QR code with employee contact details.
        /// </returns>
        /// <response code="200">QR code generated successfully and returned as an image file.</response>
        /// <response code="400">Invalid request or phone number not found.</response>
        /// <response code="500">An internal server error occurred while processing the request.</response>
        [HttpPost("{phone}/qrcode")]
        [SwaggerOperation(Summary = "Gets the QR code of an employee's contact details")]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContactQrCode([FromRoute] string phone)
        {
            var response = await _services.GetContactQrCode(phone);
            if (response.ResponseCode == ResponseCodes.SUCCESS){return File(response.Data, "image/png");}
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR){return BadRequest(response);}
            return StatusCode(500, response);
        }
    }
}