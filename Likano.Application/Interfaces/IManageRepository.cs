using Likano.Application.DTOs;
using Likano.Domain.Entities;
using Likano.Domain.Enums;
using Likano.Domain.Enums.File;

namespace Likano.Application.Interfaces
{
    public interface IManageRepository
    {
        Task<Product?> GetProduct(int id);
        Task<List<Product>?> GetAllProducts();
        Task<bool> ChangeStatus(int id, ProductStatus status);
        Task<List<CategoryDtoForManage>?> Categories(string? searchString, int? id);
        Task<List<Category>?> GetAllCategories();
        Task<Category?> GetCategory(int id);
        Task<bool> ChangeActiveStatusCategory(int id);
        Task<bool> ChangeCategory(int productId, int newCategoryId);
        Task<bool> ChangeBrand(int productId, int newBrandId);
        Task<bool> ChangeCountry(int productId, int newCountryId);
        Task<Brand?> GetBrand(int id);
        Task<List<Brand>?> GetAllBrands();
        Task<bool> ChangeActiveStatusBrand(int id);
        Task<int> AddCategoryAsync(Category category);
        Task<bool> EditCategoryAsync(int categoryId, string name, string? description);
        Task<int> AddBrandAsync(Brand brand);
        Task<bool> EditBrandAsync(int brandId, string name, string? description);
        Task<bool> AddProducerCountry(ProducerCountry country);
        Task<bool> EditProducerCountry(int countryId, string name);
        Task<ProducerCountry?> GetProducerCountry(int id);
        Task<List<ProducerCountry>?> GetAllProducerCountries();
        Task<bool> DeleteProducerCountry(int countryId);
        Task<int> AddProductAsync(Product product);
        //ფაილებისთვის - ფოტოებისთვის:
        Task<FileDto> UploadFileAsync(string? fileName, string? fileUrl, FileType? fileType, int? brandId, int? categoryId, int? productId, int? userId);
        Task<Likano.Domain.Entities.File?> GetFileAsync(int id);
        Task<bool> DeleteFileAsync(int fileId);
        Task EditFile(Likano.Domain.Entities.File? file);
        Task<List<Likano.Domain.Entities.File>?> GetAllFiles();
        Task<bool> DeleteImage(int? categoryId, int? brandId, int? productId);
        Task<bool> UpdateBrandLogoAsync(int brandId, string? logoUrl);
        Task<bool> UpdateCategoryLogoAsync(int categoryId, string? logoUrl);
    }
}
