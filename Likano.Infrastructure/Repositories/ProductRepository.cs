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

        public async Task<List<Product>?> GetAllForSearch()
        {
            return await _db.Products
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.ProducerCountry)
                .Include(x => x.Images.Where(img => !img.IsDeleted))
                .Where(x =>
                    x.Status == Domain.Enums.ProductStatus.Active &&
                    x.Category != null && x.Category.IsActive == true &&
                    x.Brand != null && x.Brand.IsActive == true
                )
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
