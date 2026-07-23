using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UrbanNoodle.Dto;
using UrbanNoodle.Services.Interface;
using UrbanNoodle.Utils;

namespace UrbanNoodle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IAlService _alService;
        private readonly ILogger<ChatController> _logger;
        public ChatController(IAlService alService, ILogger<ChatController> logger)
        {
            _alService = alService;
            _logger = logger;
        }

        [HttpPost]
        [EnableRateLimiting("ChatbotPolicy")]
        public async Task<ApiResponse> chatAl(PartDto request)
        {

            int? accountId = User.GetAccountId();

            var response = await _alService.ChatAsync(request.text, accountId);

            return response;
        }
    }
}
