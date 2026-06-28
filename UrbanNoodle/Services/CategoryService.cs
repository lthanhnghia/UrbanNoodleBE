using UrbanNoodle.ApplicationContext;

namespace UrbanNoodle.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }
        //public async Task<ApiResponse> CreateCategoryAsync(CategoryDto request)
        //{
        //    if(await _context.Category.AnyAsync(ct => ct.Name== request.Name))
        //    {
        //        throw new BadRequestException("Tên loại phân loại bị trùng");
        //    }
        //    var category = new Category
        //    {
        //        Name = request.Name,
        //        SearchName = UtilService.NormalizeText(request.Name),
        //        Description = request.Description,
        //        IsDeleted = false,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.Category.Add(category);
        //    await _context.SaveChangesAsync();
        //    return new ApiResponse(200, "Thêm mới thành công");
        //}

        //public async Task<ApiResponse> DeleteCategoryAsync(int id)
        //{
        //    var category = await _context.Category.FindAsync(id);
        //    if (category == null)
        //    {
        //        throw new NotFoundException("Không có phân loại đồ ăn này");
        //    }
        //    category.IsDeleted = true;
        //    category.Name = category.Name + category.Id;
        //    _context.Category.Update(category);
        //    await _context.SaveChangesAsync();
        //    return new ApiResponse(200, "Xóa thành công");
        //}



        //public async Task<ListCategoryDto> GetCategoryAsync(int lastId, int size,  string? key)
        //{
        //   var query = _context.Category.OrderBy(ct => ct.Id)
        //        .Where(ct => ct.Id > lastId && ct.IsDeleted == false);

        //    if (!string.IsNullOrEmpty(key))
        //    {
        //        string seachname = UtilService.NormalizeText(key);
        //        query = query.Where(ct => ct.SearchName.Contains(seachname));
        //    }
        //    var category = await query.Take(size).
        //        Select(ct => new GetCategoryDto
        //        {
        //            Id = ct.Id,
        //            Name = ct.Name,
        //            Description = ct.Description
        //        }).ToListAsync();
        //    bool hasMore = category.Count == size;
        //    return new ListCategoryDto(category,hasMore);
        //}

        //public async Task<IEnumerable<CategoryOption>> GetOptionCategoryAsync()
        //{
        //   var category =  _context.Category.OrderBy(ct=> ct.Id).Where(ct=>ct.IsDeleted==false)
        //        .Select(ct => new CategoryOption(ct.Id,ct.Name));
        //    return category;
        //}

        //public async Task<ApiResponse> UpdateCategoryAsync(int id, CategoryDto request)
        //{
        //    var category = await _context.Category.FindAsync(id);
        //    if (category == null) { 
        //         throw new NotFoundException("Không có phân loại đồ ăn này");
        //    }

        //    category.Name = request.Name;
        //    category.SearchName = UtilService.NormalizeText(request.Name);
        //    category.Description = request.Description;
        //    category.UpdatedAt = DateTime.UtcNow;
        //    _context.Category.Update(category);
        //    await _context.SaveChangesAsync();
        //    return new ApiResponse(200, "Cập nhật thành công");
        //}


    }
}
