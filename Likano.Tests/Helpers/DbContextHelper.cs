using Likano.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Likano.Tests.Helpers
{
    public static class DbContextHelper
    {
        public static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
