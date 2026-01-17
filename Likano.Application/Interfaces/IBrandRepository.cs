using Likano.Domain.Entities;

namespace Likano.Application.Interfaces
{
    public interface IBrandRepository
    {
        Task<Brand?> Get(int id);
        Task<List<Brand>?> GetAll();
    }
}
