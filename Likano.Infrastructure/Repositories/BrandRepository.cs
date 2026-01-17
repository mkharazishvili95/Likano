using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Likano.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        readonly ApplicationDbContext _db;
        public BrandRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Brand?> Get(int id) => await _db.Brands.FindAsync(id);

        public async Task<List<Brand>?> GetAll() => await _db.Brands.AsNoTracking().ToListAsync();
    }
}
