using Application.Features.Constants;
using Application.Interfaces.Application;
using Application.Services;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly IDemoService<DemoDTO> _demoService;

        public DemoController(IDemoService<DemoDTO> demoService)
        {
            _demoService = demoService;
        }

        [HttpGet("demo")]
        public async Task<IActionResult> GetRequest()
        {
            var response = await _demoService.GetDemo();
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            return BadRequest(response.Message);
        }

        [HttpGet("demo-by-id")]
        public async Task<IActionResult> GetRequestMerchantId(string merchantid)
        {
            var response = await _demoService.GetDemoById(merchantid);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            return BadRequest(response.Message);
        }

        [HttpPost("demo")]
        public async Task<IActionResult> CreateRequest(DemoDTO request)
        {
            var response = await _demoService.CreateDemo(request);
            if (response.ResponseCode == ResponseCodes.CREATED) { return Ok(response); }
            return BadRequest(response.Message);
        }

        [HttpPatch("demo")]
        public async Task<IActionResult> UpdateRequest(DemoDTO request)
        {
            var response = await _demoService.UpdateDemo(request);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            return BadRequest(response.Message);
        }

        [HttpDelete("demo-by-id")]
        public async Task<IActionResult> DeleteRequest(string id)
        {
            var response = await _demoService.DeleteDemo(id);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            return BadRequest(response.Message);
        }
    }
}
