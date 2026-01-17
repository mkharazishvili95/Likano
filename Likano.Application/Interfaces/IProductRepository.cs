using Likano.Domain.Entities;

namespace Likano.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> Get(int id);
        Task<List<Product>?> GetAll();
    }
}
