using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Likano.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Product?> Get(int id) 
            => await _db.Products.FirstOrDefaultAsync(x => x.Id == id && x.Status == Domain.Enums.ProductStatus.Active);

        public async Task<List<Product>?> GetAll() => await _db.Products
            .Where(x => x.Status == Domain.Enums.ProductStatus.Active)
            .AsNoTracking()
            .ToListAsync();
    }
}
