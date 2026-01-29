using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Tests.Helpers;

namespace Likano.Tests.Repositories
{
    [TestFixture]
    public class CategoryRepositoryTests
    {
        ApplicationDbContext _db = null!;
        CategoryRepository _repo = null!;
        int _activeCategoryId;
        int _inactiveCategoryId;
        int _nullStatusCategoryId;

        [SetUp]
        public async Task SetUp()
        {
            _db = DbContextHelper.GetInMemoryDbContext();
            _repo = new CategoryRepository(_db);

            var activeCategory = new Category
            {
                Name = "Active Category",
                Description = "Active Category Description",
                Logo = "https://category-logo.jpg",
                IsActive = true
            };
            _db.Categories.Add(activeCategory);
            await _db.SaveChangesAsync();
            _activeCategoryId = activeCategory.Id;

            var inactiveCategory = new Category
            {
                Name = "Inactive Category",
                Description = "Inactive Category Description",
                IsActive = false
            };
            _db.Categories.Add(inactiveCategory);
            await _db.SaveChangesAsync();
            _inactiveCategoryId = inactiveCategory.Id;

            var nullStatusCategory = new Category
            {
                Name = "Null Status Category",
                Description = "Category without status",
                IsActive = null
            };
            _db.Categories.Add(nullStatusCategory);
            await _db.SaveChangesAsync();
            _nullStatusCategoryId = nullStatusCategory.Id;
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        #region Get Tests

        [Test]
        public async Task Get_ExistingCategory_ReturnsCategory()
        {
            var result = await _repo.Get(_activeCategoryId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Active Category"));
            Assert.That(result.IsActive, Is.True);
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

        [Test]
        public async Task Get_InactiveCategory_ReturnsCategory()
        {
            var result = await _repo.Get(_inactiveCategoryId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Inactive Category"));
            Assert.That(result.IsActive, Is.False);
        }

        #endregion

        #region GetAll Tests

        [Test]
        public async Task GetAll_ReturnsOnlyActiveCategories()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(1));
            Assert.That(result.All(c => c.IsActive == true), Is.True);
        }

        [Test]
        public async Task GetAll_DoesNotReturnInactiveCategories()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Any(c => c.IsActive == false), Is.False);
        }

        [Test]
        public async Task GetAll_DoesNotReturnNullStatusCategories()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Any(c => c.IsActive == null), Is.False);
        }

        [Test]
        public async Task GetAll_NoActiveCategories_ReturnsEmptyList()
        {
            var activeCategory = await _db.Categories.FindAsync(_activeCategoryId);
            activeCategory!.IsActive = false;
            await _db.SaveChangesAsync();

            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAll_MultipleActiveCategories_ReturnsAll()
        {
            var newActiveCategory = new Category
            {
                Name = "Second Active Category",
                Description = "Description",
                IsActive = true
            };
            _db.Categories.Add(newActiveCategory);
            await _db.SaveChangesAsync();

            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAll_ReturnsCorrectCategoryData()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            var category = result!.First();
            Assert.That(category.Name, Is.EqualTo("Active Category"));
            Assert.That(category.Description, Is.EqualTo("Active Category Description"));
            Assert.That(category.Logo, Is.EqualTo("https://category-logo.jpg"));
        }

        #endregion
    }
}