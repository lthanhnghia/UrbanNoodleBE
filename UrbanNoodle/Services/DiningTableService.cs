using Microsoft.EntityFrameworkCore;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.DiningTable;
using UrbanNoodle.Entities;
using UrbanNoodle.Exceptions;
using UrbanNoodle.Services.Interface;
using UrbanNoodle.Utils;

namespace UrbanNoodle.Services
{
    public class DiningTableService: IDiningTable
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DiningTableService> _logger;
        public DiningTableService(ApplicationDbContext context, ILogger<DiningTableService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse> CreateDiningTableAsync(CreateDiningTableDto request)
        {
           
            var diningTable = new DiningTable()
            {
                Name = request.Name,
                SearchName = UtilService.NormalizeText(request.Name),
                Status = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
           
           _context.DiningTable.Add(diningTable);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Thêm mới thành công");
        }

        public async Task<ApiResponse> DeleteDiningTableAsync(int id)
        {
            var diningTable = await _context.DiningTable.FindAsync(id);
            if (diningTable == null)
            {
                throw new NotFoundException("Bàn ăn này không có trong hệ thống");
            }
            diningTable.IsDeleted = true;
            _context.DiningTable.Update(diningTable);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Xóa thành công");
        }

        public async Task<IEnumerable<GetDiningTableDto>> GetDiningTableAsync
            (int lastId, int size, bool isDelete, bool? status, string? key)
        {
            var query = _context.DiningTable
                .Where(dt => dt.Id > lastId && dt.IsDeleted == isDelete &&
                (!status.HasValue || dt.Status == status.Value));

            if (!string.IsNullOrEmpty(key))
            {
                string seachname = UtilService.NormalizeText(key);
                query = query.Where(dt => dt.SearchName.Contains(seachname));
            }
            var diningtable = await query.OrderBy(dt => dt.Id).Take(size)
                .Select(dt => new GetDiningTableDto
                {
                    Id = dt.Id,
                    Name = dt.Name,
                    Status = dt.Status
                }).ToListAsync();
            return diningtable;
        }

        public async Task<ApiResponse> UpdateDiningTableAsync(int id, UpdateDiningTableDto request)
        {
            var diningTable = await _context.DiningTable.FindAsync(id);
            if(diningTable == null)
            {
                throw new NotFoundException("Bàn ăn này không có trong hệ thống");
            }
            diningTable.Name = request.Name;
            diningTable.SearchName = UtilService.NormalizeText(diningTable.SearchName);
            diningTable.Status = request.Status;
            diningTable.UpdatedAt = DateTime.UtcNow;
            _context.DiningTable.Update(diningTable);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Cập nhật thành công");

        }
    }
}
