using Likano.Domain.Entities;

namespace Likano.Application.Interfaces
{
    public interface IProducerCountryRepository
    {
        Task<List<ProducerCountry>?> GetAll();
        Task<ProducerCountry?> Get(int id);
    }
}
