using ClinicBookingMVC.Models;
using ClinicBookingMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBookingMVC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public ChatController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("Ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return BadRequest(new ChatResponse { IsError = true, ErrorMessage = "Tin nhắn không hợp lệ." });
            }

            var response = await _geminiService.GetDoctorRecommendationAsync(request.Message);
            return Ok(response);
        }
    }
}
