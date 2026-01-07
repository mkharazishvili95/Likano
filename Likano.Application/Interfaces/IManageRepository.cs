using Likano.Application.DTOs;
using Likano.Domain.Entities;
using Likano.Domain.Enums;

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
    }
}
