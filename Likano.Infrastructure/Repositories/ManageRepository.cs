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

        public async Task<Product?> GetProduct(int id) => await _db.Products
            .Include(x => x.Category)
            .Include(x => x.ProducerCountry)
            .Include(x => x.Brand).
            FirstOrDefaultAsync(x => x.Id == id);

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
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task EditFile(Domain.Entities.File? file)
        {
            if (file != null && !file.IsDeleted)
                _db.Update(file);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Likano.Domain.Entities.File>?> GetAllFiles()
        {
            return await _db.Files
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category.Id;
        }

        public async Task<bool> UpdateCategoryLogoAsync(int categoryId, string? logoUrl)
        {
            var cat = await _db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (cat == null) return false;
            cat.Logo = logoUrl;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditCategoryAsync(int categoryId, string name, string? description)
        {
            var cat = await _db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (cat is null) return false;

            cat.Name = name;
            cat.Description = description;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteImage(int? categoryId, int? brandId, int? productId)
        {
            if (categoryId.HasValue)
            {
                var category = await _db.Categories.FindAsync(categoryId.Value);
                if (category == null) return false;
                category.Logo = null;
            }
            else if (brandId.HasValue)
            {
                var brand = await _db.Brands.FindAsync(brandId.Value);
                if (brand == null) return false;
                brand.Logo = null;
            }
            else if (productId.HasValue)
            {
                var product = await _db.Products.FindAsync(productId.Value);
                if (product == null) return false;
                product.ImageUrl = null;
            }
            else
            {
                return false;
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> AddBrandAsync(Brand brand)
        {
            _db.Brands.Add(brand);
            await _db.SaveChangesAsync();
            return brand.Id;
        }

        public async Task<bool> UpdateBrandLogoAsync(int brandId, string? logoUrl)
        {
            var brand = await _db.Brands.FirstOrDefaultAsync(b => b.Id == brandId);
            if (brand == null) return false;
            brand.Logo = logoUrl;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditBrandAsync(int brandId, string name, string? description)
        {
            var brand = await _db.Brands.FirstOrDefaultAsync(b => b.Id == brandId);
            if (brand is null) return false;

            brand.Name = name;
            brand.Description = description;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<ProducerCountry?> GetProducerCountry(int id) => await _db.ProducerCountries.FirstOrDefaultAsync(c => c.Id == id);
        public async Task<List<ProducerCountry>?> GetAllProducerCountries() => await _db.ProducerCountries.ToListAsync();
        public async Task<bool> AddProducerCountry(ProducerCountry country)
        {
            if(country == null)
                return false;

            await _db.ProducerCountries.AddAsync(country);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditProducerCountry(int countryId, string name)
        {
           var country = await _db.ProducerCountries.FirstOrDefaultAsync(c => c.Id == countryId);
              if (country == null)
                 return false;
    
                country.Name = name;
                await _db.SaveChangesAsync();
                return true;
        }

        public async Task<bool> DeleteProducerCountry(int countryId)
        {
            var country = await _db.ProducerCountries
                .FirstOrDefaultAsync(c => c.Id == countryId);

            if (country == null)
                return false;

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var products = await _db.Products
                    .Where(p => p.ProducerCountryId == countryId)
                    .ToListAsync();

                foreach (var product in products)
                {
                    product.ProducerCountryId = null;
                }

                _db.ProducerCountries.Remove(country);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> ChangeCountry(int productId, int newCountryId)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null)
                return false;

            var newCountry = await _db.ProducerCountries.FindAsync(newCountryId);
            if (newCountry == null)
                return false;

            product.ProducerCountryId = newCountryId;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> AddProductAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return product.Id;
        }
    }
}