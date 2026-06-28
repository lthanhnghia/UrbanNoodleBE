using Microsoft.EntityFrameworkCore;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Food;
using UrbanNoodle.Entities;
using UrbanNoodle.Exceptions;
using UrbanNoodle.Services.Interface;
using UrbanNoodle.Utils;
namespace UrbanNoodle.Services
{
    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FoodService> _logger;
        public FoodService(ApplicationDbContext context, ILogger<FoodService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ApiResponse> CreateFoodAsync(CreateFoodDto request)
        {
            var food = new Food()
            {
                FoodName = request.FoodName,
                Price = request.Price,
                IsDeleted = false,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow,
                SearchName = UtilService.NormalizeText(request.FoodName)
            };

            string imageUrl = await ImageName(request.Image);
            if (imageUrl == null)
            {
                throw new BadRequestException("File ảnh không hợp lệ");
            }
            food.ImageUrl = imageUrl;
            _logger.LogInformation("image = " + food.ImageUrl);
            _context.Food.Add(food);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Thêm mới thành công");
        }

        public async Task<ApiResponse> DeleteFoodAsync(int id)
        {
            var food = await _context.Food.FindAsync(id);
            if (food == null)
            {
                throw new NotFoundException("Không có đồ ăn này trong hệ thống");
            }
            food.IsDeleted = true;
            _context.Food.Update(food);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Xóa thành công đồ ăn");
        }

        public async Task<IEnumerable<GetFoodDto>> GetAllFood()
        {
            var query = await _context.Food.OrderBy(fd => fd.Id)
                 .Select(fd => new GetFoodDto
                 {
                     Id = fd.Id,
                     Name = fd.FoodName,
                     Price = fd.Price,
                     image = fd.ImageUrl,
                     CategoryName = fd.Category.CategoryName
                 }).ToListAsync();
            return query;
        }

        public async Task<ListFood> GetFoodAsync(int lastId, int size, string? key)
        {
            var query = _context.Food.OrderBy(fd => fd.Id)
                .Where(fd => fd.Id > lastId && fd.IsDeleted == false);

            if (!string.IsNullOrEmpty(key))
            {
                string seachname = UtilService.NormalizeText(key);
                query = query.Where(fd => fd.SearchName.Contains(seachname));
            }
            var food = await query.Take(size)
                .Select(fd => new GetFoodDto
                {
                    Id = fd.Id,
                    Name = fd.FoodName,
                    Price = fd.Price,
                    image = fd.ImageUrl,
                    CategoryName = fd.Category.CategoryName
                }).ToListAsync();
            bool hasMore = food.Count == size;
            return new ListFood(food, hasMore);
        }

        public async Task<ApiResponse> UpdateFoodAsync(int id, UpdateFoodDto request)
        {
            var food = await _context.Food.FindAsync(id);
            if (food == null)
            {
                throw new NotFoundException("Không có đồ ăn này trong cửa hàng");
            }
            food.FoodName = request.Name;
            food.SearchName = UtilService.NormalizeText(request.Name);
            food.Price = request.Price;
            food.UpdatedAt = DateTime.UtcNow;
            food.CategoryId = request.CategoryId;

            if (request.Image != null)
            {
                var imageUrl = await ImageName(request.Image);
                if (imageUrl == null)
                {
                    throw new BadRequestException("File ảnh không hợp lệ");
                }
                food.ImageUrl = imageUrl;
            }
            _logger.LogInformation("image = " + food.ImageUrl);
            _context.Food.Update(food);
            await _context.SaveChangesAsync();

            return new ApiResponse(200, "Cập nhật thành công");
        }

        private async Task<string> ImageName(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BadRequestException("File ảnh không hợp lệ");
            }

            var allowedExtentions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extention = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtentions.Contains(extention))
            {
                throw new BadRequestException("File ảnh chỉ cho phép jpg, jpeg, png, webp");
            }

            var fileName = Guid.NewGuid() + extention;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "foods");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/foods/{fileName}";
        }
    }
}
