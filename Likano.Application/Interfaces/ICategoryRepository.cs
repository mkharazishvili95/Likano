using Likano.Domain.Entities;

namespace Likano.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> Get(int id);
        Task<List<Category>?> GetAll();
    }
}
