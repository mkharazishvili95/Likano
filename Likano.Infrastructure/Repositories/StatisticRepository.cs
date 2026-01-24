using Likano.Application.Interfaces;
using Likano.Infrastructure.Data;

namespace Likano.Infrastructure.Repositories
{
    public class StatisticRepository : IStatisticRepository
    {
        readonly ApplicationDbContext _db;
        public StatisticRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null) return;

            product.ViewCount++;
            await _db.SaveChangesAsync();
        }
    }
}
