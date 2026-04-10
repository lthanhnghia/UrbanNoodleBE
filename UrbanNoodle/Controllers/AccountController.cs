using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Account;
using UrbanNoodle.Service;
using UrbanNoodle.Service.Interface;

namespace UrbanNoodle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IAccountService _accountServices;
        private readonly ILogger<AccountService> _logger;
        public AccountController(IAccountService accountServices, ILogger<AccountService> logger )
        {
            _accountServices = accountServices;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateAccount([FromBody]AccountDTO request)
        {

            // var result = await _accountServices.CreateAccountAsync(request);
            _logger.LogInformation($"{request.FullName}");
            return new ApiResponse(200, "result.Description");
        }

        [HttpGet]
        public async Task<ListAccountDto> GetAccount(
        [FromQuery] int lastId = 0,
        [FromQuery] int size = 3,
        [FromQuery] bool isDelete = false,
        [FromQuery] string? key = null)
        {

            return await _accountServices.GetAccountAsync(lastId, size, isDelete,key);
        }

        [HttpPut("{ID}")]
        public async Task<ActionResult<ApiResponse>> UpdateAccount(int ID, [FromBody] UpdateAccountDto request)
        {
            var result = await _accountServices.UpdateAccountAsync(ID,request);
            return new ApiResponse(result.Status, result.Description);

        }

        [HttpDelete("{ID}")]
        public async Task<ActionResult<ApiResponse>> DeleteAccount(int ID)
        {
            var result = await _accountServices.DeleteAccountAsync(ID);
            return new ApiResponse(result.Status, result.Description);

        }
    }
}
