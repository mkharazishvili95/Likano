using Likano.Application.Interfaces;

namespace Likano.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
