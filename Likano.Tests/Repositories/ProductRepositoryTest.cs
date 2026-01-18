using Likano.Domain.Entities;
using Likano.Domain.Enums;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Tests.Helpers;
using NUnit.Framework;

namespace Likano.Tests.Repositories
{
    [TestFixture]
    public class ProductRepositoryTest
    {
        ApplicationDbContext _db = null!;
        ProductRepository _repo = null!;
        int _activeProductId;
        int _deletedProductId;
        int _hiddenProductId;
        int _categoryId;

        [SetUp]
        public async Task SetUp()
        {
            _db = DbContextHelper.GetInMemoryDbContext();
            _repo = new ProductRepository(_db);

            var category = new Category
            {
                Name = "Test Category",
                Description = "Test Description",
                IsActive = true
            };
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            _categoryId = category.Id;

            var activeProduct = new Product
            {
                Title = "Active Product",
                Description = "Active Product Description",
                Price = 100,
                IsAvailable = true,
                CategoryId = _categoryId,
                Status = ProductStatus.Active,
                CreateDate = DateTime.UtcNow
            };
            _db.Products.Add(activeProduct);
            await _db.SaveChangesAsync();
            _activeProductId = activeProduct.Id;

            var deletedProduct = new Product
            {
                Title = "Deleted Product",
                Description = "Deleted Product Description",
                Price = 200,
                IsAvailable = false,
                CategoryId = _categoryId,
                Status = ProductStatus.Deleted,
                CreateDate = DateTime.UtcNow
            };
            _db.Products.Add(deletedProduct);
            await _db.SaveChangesAsync();
            _deletedProductId = deletedProduct.Id;

            var hiddenProduct = new Product
            {
                Title = "Hidden Product",
                Description = "Hidden Product Description",
                Price = 150,
                IsAvailable = true,
                CategoryId = _categoryId,
                Status = ProductStatus.Hiden,
                CreateDate = DateTime.UtcNow
            };
            _db.Products.Add(hiddenProduct);
            await _db.SaveChangesAsync();
            _hiddenProductId = hiddenProduct.Id;
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        #region Get Tests

        [Test]
        public async Task Get_ActiveProduct_ReturnsProduct()
        {
            var result = await _repo.Get(_activeProductId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo("Active Product"));
            Assert.That(result.Status, Is.EqualTo(ProductStatus.Active));
        }

        [Test]
        public async Task Get_DeletedProduct_ReturnsNull()
        {
            var result = await _repo.Get(_deletedProductId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Get_HiddenProduct_ReturnsNull()
        {
            var result = await _repo.Get(_hiddenProductId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Get_NonExistingId_ReturnsNull()
        {
            var result = await _repo.Get(9999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Get_ZeroId_ReturnsNull()
        {
            var result = await _repo.Get(0);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Get_NegativeId_ReturnsNull()
        {
            var result = await _repo.Get(-1);

            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetAll Tests

        [Test]
        public async Task GetAll_ReturnsOnlyActiveProducts()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(1));
            Assert.That(result.All(p => p.Status == ProductStatus.Active), Is.True);
        }

        [Test]
        public async Task GetAll_DoesNotReturnDeletedProducts()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Any(p => p.Status == ProductStatus.Deleted), Is.False);
        }

        [Test]
        public async Task GetAll_DoesNotReturnHiddenProducts()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Any(p => p.Status == ProductStatus.Hiden), Is.False);
        }

        [Test]
        public async Task GetAll_NoActiveProducts_ReturnsEmptyList()
        {
            var activeProduct = await _db.Products.FindAsync(_activeProductId);
            activeProduct!.Status = ProductStatus.Deleted;
            await _db.SaveChangesAsync();

            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAll_MultipleActiveProducts_ReturnsAll()
        {
            var newActiveProduct = new Product
            {
                Title = "Second Active Product",
                Description = "Description",
                Price = 300,
                CategoryId = _categoryId,
                Status = ProductStatus.Active,
                CreateDate = DateTime.UtcNow
            };
            _db.Products.Add(newActiveProduct);
            await _db.SaveChangesAsync();

            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAll_ReturnsProductsWithCorrectData()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            var product = result!.First();
            Assert.That(product.Title, Is.EqualTo("Active Product"));
            Assert.That(product.Price, Is.EqualTo(100));
            Assert.That(product.CategoryId, Is.EqualTo(_categoryId));
        }

        #endregion
    }
}