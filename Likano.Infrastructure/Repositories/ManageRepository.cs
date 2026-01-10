using Likano.Application.DTOs;
using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using Likano.Domain.Enums;
using Likano.Domain.Enums.File;
using Likano.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Likano.Infrastructure.Repositories
{
    public class ManageRepository : IManageRepository
    {
        readonly ApplicationDbContext _db;
        public ManageRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Product?> GetProduct(int id) => await _db.Products.Include(x => x.Category).Include(x => x.Brand).FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<Product>?> GetAllProducts() => await _db.Products.ToListAsync();
        public async Task<List<Category>?> GetAllCategories() => await _db.Categories.ToListAsync();
        //Product Status Change:
        public async Task<bool> ChangeStatus(int id, ProductStatus status)
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null)
                return false;

            product.Status = status;
            product.UpdateDate = DateTime.UtcNow.AddHours(4);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<CategoryDtoForManage>?> Categories(string? searchString, int? id)
        {
            var query = _db.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString.Trim(), StringComparison.OrdinalIgnoreCase));
            }
            if (id.HasValue)
            {
                query = query.Where(c => c.Id == id.Value);
            }
            var categories = await query
                .Select(c => new CategoryDtoForManage
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
            return categories.Count > 0 ? categories : null;
        }

        public async Task<bool> ChangeActiveStatusCategory(int id)
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null)
                return false;

            category.IsActive = category.IsActive.HasValue && category.IsActive.Value ? false : true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Category?> GetCategory(int id) => await _db.Categories.FindAsync(id);

        public async Task<bool> ChangeCategory(int productId, int newCategoryId)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null)
                return false;

            product.CategoryId = newCategoryId;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeBrand(int productId, int newBrandId)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null)
                return false;

            product.BrandId = newBrandId;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Brand?> GetBrand(int id) => await _db.Brands.FindAsync(id);

        public async Task<List<Brand>?> GetAllBrands() => await _db.Brands.ToListAsync();

        public async Task<bool> ChangeActiveStatusBrand(int id)
        {
            var brand = await _db.Brands.FindAsync(id);

            if (brand == null)
                return false;

            brand.IsActive = brand.IsActive == true ? false : true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<FileDto> UploadFileAsync(string? fileName, string? fileUrl, FileType? fileType, int? brandId, int? categoryId, int? productId, int? userId)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileUrl) || fileType == null)
                throw new ArgumentException("Invalid file data");

            var fileEntity = new Likano.Domain.Entities.File
            {
                FileName = fileName,  
                FileUrl = fileUrl,
                FileType = fileType.Value,
                UploadDate = DateTime.UtcNow.AddHours(4),
                DeleteDate = null,
                IsDeleted = false,
                BrandId = brandId,
                CategoryId = categoryId,
                ProductId = productId,
                UserId = userId
            };

            _db.Files.Add(fileEntity);
            await _db.SaveChangesAsync();

            return new FileDto
            {
                Id = fileEntity.Id,
                FileName = fileEntity.FileName,
                FileUrl = fileEntity.FileUrl,
                FileType = fileEntity.FileType
            };
        }

        public async Task<Domain.Entities.File?> GetFileAsync(int id)
        {
            var file = await _db.Files.FindAsync(id);

            if (file == null || file.IsDeleted)
                return null;

            return file;
        }

        public async Task<bool> DeleteFileAsync(int fileId)
        {
            var file = await GetFileAsync(fileId);

            if (file == null)
                return false;

            if (file.IsDeleted)
                return false;

            file.IsDeleted = true;
            file.DeleteDate = DateTime.UtcNow.AddHours(4);

            _db.Files.Update(file);
            return true;
        }

        public async Task EditFile(Domain.Entities.File? file)
        {
            if (file != null && !file.IsDeleted)
                _db.Update(file);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Likano.Domain.Entities.File>?> GetAll()
        {
            return await _db.Files
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }
    }
}