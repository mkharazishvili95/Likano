using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Likano.Infrastructure.Repositories
{
    public class ManageRepository : IManageRepository
    {
        readonly ApplicationDbContext _db;
        public ManageRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Product?> GetProduct(int id) => await _db.Products.FindAsync(id);

        public async Task<List<Product>?> GetAllProducts() => await _db.Products.ToListAsync();
    }
}