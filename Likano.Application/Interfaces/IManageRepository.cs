using Likano.Domain.Entities;

namespace Likano.Application.Interfaces
{
    public interface IManageRepository
    {
        Task<Product?> GetProduct(int id);
        Task<List<Product>?> GetAllProducts();
    }
}
