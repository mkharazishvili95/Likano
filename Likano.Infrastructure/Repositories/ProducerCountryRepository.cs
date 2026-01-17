using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Likano.Infrastructure.Repositories
{
    public class ProducerCountryRepository : IProducerCountryRepository
    {
        readonly ApplicationDbContext _db;
        public ProducerCountryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ProducerCountry?> Get(int id) => await _db.ProducerCountries.FindAsync(id);

        public async Task<List<ProducerCountry>?> GetAll() => await _db.ProducerCountries.AsNoTracking().ToListAsync();
    }
}
