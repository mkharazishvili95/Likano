using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Tests.Helpers;

namespace Likano.Tests.Repositories
{
    [TestFixture]
    public class BrandRepositoryTests
    {
        ApplicationDbContext _db = null!;
        BrandRepository _repo = null!;
        int _activeBrandId;
        int _inactiveBrandId;

        [SetUp]
        public async Task SetUp()
        {
            _db = DbContextHelper.GetInMemoryDbContext();
            _repo = new BrandRepository(_db);

            var activeBrand = new Brand
            {
                Name = "Active Brand",
                Description = "Active Brand Description",
                Logo = "https://logo.jpg",
                IsActive = true
            };
            _db.Brands.Add(activeBrand);
            await _db.SaveChangesAsync();
            _activeBrandId = activeBrand.Id;

            var inactiveBrand = new Brand
            {
                Name = "Inactive Brand",
                Description = "Inactive Brand Description",
                IsActive = false
            };
            _db.Brands.Add(inactiveBrand);
            await _db.SaveChangesAsync();
            _inactiveBrandId = inactiveBrand.Id;
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        #region Get Tests

        [Test]
        public async Task Get_ExistingBrand_ReturnsBrand()
        {
            var result = await _repo.Get(_activeBrandId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Active Brand"));
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
        public async Task Get_InactiveBrand_ReturnsBrand()
        {
            var result = await _repo.Get(_inactiveBrandId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Inactive Brand"));
            Assert.That(result.IsActive, Is.False);
        }

        #endregion

        #region GetAll Tests

        [Test]
        public async Task GetAll_ReturnsOnlyActiveBrands()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(1));
            Assert.That(result.All(b => b.IsActive), Is.True);
        }

        [Test]
        public async Task GetAll_DoesNotReturnInactiveBrands()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Any(b => !b.IsActive), Is.False);
        }

        [Test]
        public async Task GetAll_NoActiveBrands_ReturnsEmptyList()
        {
            var activeBrand = await _db.Brands.FindAsync(_activeBrandId);
            activeBrand!.IsActive = false;
            await _db.SaveChangesAsync();

            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAll_MultipleActiveBrands_ReturnsAll()
        {
            var newActiveBrand = new Brand
            {
                Name = "Second Active Brand",
                Description = "Description",
                IsActive = true
            };
            _db.Brands.Add(newActiveBrand);
            await _db.SaveChangesAsync();

            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAll_ReturnsCorrectBrandData()
        {
            var result = await _repo.GetAll();

            Assert.That(result, Is.Not.Null);
            var brand = result!.First();
            Assert.That(brand.Name, Is.EqualTo("Active Brand"));
            Assert.That(brand.Description, Is.EqualTo("Active Brand Description"));
            Assert.That(brand.Logo, Is.EqualTo("https://logo.jpg"));
        }

        #endregion
    }
}