using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Likano.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Category?> Get(int id) => await _db.Categories.FindAsync(id);
        public async Task<List<Category>?> GetAll() => await _db.Categories.AsNoTracking().ToListAsync(); 
    }
}
