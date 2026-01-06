using Likano.Domain.Entities;
using Likano.Domain.Enums;

namespace Likano.Application.Interfaces
{
    public interface IManageRepository
    {
        Task<Product?> GetProduct(int id);
        Task<List<Product>?> GetAllProducts();
        Task<bool> ChangeStatus(int id, ProductStatus status);
    }
}
