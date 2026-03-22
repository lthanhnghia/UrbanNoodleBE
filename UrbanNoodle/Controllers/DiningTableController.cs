using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanNoodle.Dto.Food;
using UrbanNoodle.Dto;
using UrbanNoodle.Services;
using UrbanNoodle.Services.Interface;
using UrbanNoodle.Dto.DiningTable;

namespace UrbanNoodle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiningTableController : ControllerBase
    {
        private readonly IDiningTable _diningTable;
        public DiningTableController(IDiningTable diningTable)
        {
            _diningTable = diningTable;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateDiningTable([FromBody] CreateDiningTableDto request)
        {

            var result = await _diningTable.CreateDiningTableAsync(request);
            return new ApiResponse(result.Status, result.Description);
        }
        [HttpPut("{ID}")]
        public async Task<ActionResult<ApiResponse>> UpdateDiningTable(int ID, [FromBody] UpdateDiningTableDto request)
        {
            var result = await _diningTable.UpdateDiningTableAsync(ID, request);
            return new ApiResponse(result.Status, result.Description);

        }
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ApiResponse>> DeleteFood(int ID)
        {
            var result = await _diningTable.DeleteDiningTableAsync(ID);
            return new ApiResponse(result.Status, result.Description);

        }

        [HttpGet]
        public async Task<IEnumerable<GetDiningTableDto>> GetDiningTable(
        [FromQuery] int lastId = 0,
        [FromQuery] int size = 3,
        [FromQuery] bool? status= null,
        [FromQuery] bool isDelete = false,
        [FromQuery] string? key = null)
        {

            return await _diningTable.GetDiningTableAsync(lastId, size, isDelete, status, key);
        }
    }
}
