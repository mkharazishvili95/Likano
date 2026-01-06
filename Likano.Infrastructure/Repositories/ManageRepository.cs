using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using Likano.Domain.Enums;
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

        public async Task<bool> ChangeStatus(int id, ProductStatus status)
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null)
                return false;

            product.Status = status;
            product.UpdateDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}