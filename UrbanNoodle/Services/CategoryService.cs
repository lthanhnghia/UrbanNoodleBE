using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Category;
using UrbanNoodle.Entities;
using UrbanNoodle.Exceptions;
using UrbanNoodle.Service;
using UrbanNoodle.Services.Interface;
using UrbanNoodle.Utils;

namespace UrbanNoodle.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ApiResponse> CreateCategoryAsync(CategoryDto request)
        {
            if(await _context.Category.AnyAsync(ct => ct.Name== request.Name))
            {
                throw new BadRequestException("Tên loại phân loại bị trùng");
            }
            var category = new Category
            {
                Name = request.Name,
                SearchName = UtilService.NormalizeText(request.Name),
                Description = request.Description,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
           
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Thêm mới thành công");
        }

        public async Task<ApiResponse> DeleteCategoryAsync(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                throw new NotFoundException("Không có phân loại đồ ăn này");
            }
            category.IsDeleted = true;
            _context.Category.Update(category);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Xóa thành công");
        }

        

        public async Task<IEnumerable<GetCategoryDto>> GetCategoryAsync(int lastId, int size, bool isDelete, string? key)
        {
           var query = _context.Category.Where(ct => ct.Id > lastId && ct.IsDeleted == isDelete);

            if (!string.IsNullOrEmpty(key))
            {
                string seachname = UtilService.NormalizeText(key);
                query = query.Where(ct => ct.SearchName.Contains(seachname));
            }
            var category = await query.OrderBy(ct => ct.Id).Take(size).
                Select(ct => new GetCategoryDto
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description
                }).ToListAsync();
            return category;
        }

        public async Task<IEnumerable<GetCategoryDto>> GetOptionCategoryAsync()
        {
           var category = _context.Category.Select(ct => new GetCategoryDto(ct.Id,ct.Name,ct.Description));
            return category;
        }

        public async Task<ApiResponse> UpdateCategoryAsync(int id, CategoryDto request)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null) { 
                 throw new NotFoundException("Không có phân loại đồ ăn này");
            }
            if(category.Name == request.Name)
            {
                throw new BadRequestException("Trùng tên phân loại đồ ăn");
            }
            category.Name = request.Name;
            category.SearchName = UtilService.NormalizeText(request.Name);
            category.Description = request.Description;
            category.UpdatedAt = DateTime.UtcNow;
            _context.Category.Update(category);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Cập nhật thành công");
        }

       
    }
}
