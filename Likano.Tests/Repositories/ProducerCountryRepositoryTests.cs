using Likano.Domain.Entities;
using Likano.Domain.Enums;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Tests.Helpers;

namespace Likano.Tests.Repositories
{
    [TestFixture]
    public class StatisticRepositoryTests
    {
        ApplicationDbContext _db = null!;
        StatisticRepository _repo = null!;
        int _productId;
        int _categoryId;

        [SetUp]
        public async Task SetUp()
        {
            _db = DbContextHelper.GetInMemoryDbContext();
            _repo = new StatisticRepository(_db);

            var category = new Category
            {
                Name = "Test Category",
                IsActive = true
            };
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            _categoryId = category.Id;

            var product = new Product
            {
                Title = "Test Product",
                Description = "Test Product Description",
                Price = 100,
                CategoryId = _categoryId,
                Status = ProductStatus.Active,
                CreateDate = DateTime.UtcNow,
                ViewCount = 0
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            _productId = product.Id;
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        #region AddViewCount Tests

        [Test]
        public async Task AddViewCount_ExistingProduct_IncrementsViewCount()
        {
            await _repo.AddViewCount(_productId);

            var product = await _db.Products.FindAsync(_productId);
            Assert.That(product, Is.Not.Null);
            Assert.That(product!.ViewCount, Is.EqualTo(1));
        }

        [Test]
        public async Task AddViewCount_CalledMultipleTimes_IncrementsCorrectly()
        {
            await _repo.AddViewCount(_productId);
            await _repo.AddViewCount(_productId);
            await _repo.AddViewCount(_productId);

            var product = await _db.Products.FindAsync(_productId);
            Assert.That(product, Is.Not.Null);
            Assert.That(product!.ViewCount, Is.EqualTo(3));
        }

        [Test]
        public async Task AddViewCount_NonExistingProduct_DoesNotThrowException()
        {
            Assert.DoesNotThrowAsync(async () => await _repo.AddViewCount(9999));
        }

        [Test]
        public async Task AddViewCount_ZeroId_DoesNotThrowException()
        {
            Assert.DoesNotThrowAsync(async () => await _repo.AddViewCount(0));
        }

        [Test]
        public async Task AddViewCount_NegativeId_DoesNotThrowException()
        {
            Assert.DoesNotThrowAsync(async () => await _repo.AddViewCount(-1));
        }


        [Test]
        public async Task AddViewCount_ProductWithHighViewCount_IncrementsCorrectly()
        {
            var product = await _db.Products.FindAsync(_productId);
            product!.ViewCount = 999;
            await _db.SaveChangesAsync();

            await _repo.AddViewCount(_productId);

            var updatedProduct = await _db.Products.FindAsync(_productId);
            Assert.That(updatedProduct!.ViewCount, Is.EqualTo(1000));
        }

        #endregion
    }
}