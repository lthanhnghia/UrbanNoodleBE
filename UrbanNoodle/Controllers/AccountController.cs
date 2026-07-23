using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanNoodle.Dto.Account;
using UrbanNoodle.Service.Interface;
using UrbanNoodle.Utils;

namespace UrbanNoodle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IAccountService _accountServices;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAccountService accountServices, ILogger<AccountController> logger)
        {
            _accountServices = accountServices;
            _logger = logger;
        }

        [Authorize(Roles = "client")]
        [HttpGet("historyorder")]
        public async Task<IEnumerable<HistoryOrderUserDto>> GetHistoryOrder(
        [FromQuery] int accountId,
        [FromQuery] int lastId = 0,
        [FromQuery] int size = 3)
        {
            var id = User.GetAccountId();
            _logger.LogInformation($"accountId: {id}");
            return await _accountServices.HistoryOrderUserDto(id, accountId, lastId, size);
        }
    }
}
