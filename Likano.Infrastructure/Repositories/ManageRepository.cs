using Likano.Application.DTOs;
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

        public async Task<List<CategoryDtoForManage>?> Categories(string? searchString, int? id)
        {
            var query = _db.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString.Trim(), StringComparison.OrdinalIgnoreCase));
            }
            if (id.HasValue)
            {
                query = query.Where(c => c.Id == id.Value);
            }
            var categories = await query
                .Select(c => new CategoryDtoForManage
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
            return categories.Count > 0 ? categories : null;
        }
    }
}