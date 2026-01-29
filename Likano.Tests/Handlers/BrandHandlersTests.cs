using Likano.Application.Features.Manage.Brand.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Brand.Commands.Create;
using Likano.Application.Features.Manage.Brand.Queries.Get;
using Likano.Application.Features.Manage.Brand.Queries.GetAll;
using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Tests.Helpers;
using Microsoft.Extensions.Options;

namespace Likano.Tests.Handlers
{
    [TestFixture]
    public class BrandHandlersTests
    {
        ApplicationDbContext _db = null!;
        ManageRepository _repo = null!;
        int _brandId;

        [SetUp]
        public async Task SetUp()
        {
            _db = DbContextHelper.GetInMemoryDbContext();
            var imageKitSettings = Options.Create(new Likano.Application.Configuration.ImageKitSettings());
            _repo = new ManageRepository(_db, imageKitSettings);

            var brand = new Brand
            {
                Name = "Test Brand",
                Description = "Test Brand Description",
                IsActive = true
            };
            _db.Brands.Add(brand);
            await _db.SaveChangesAsync();
            _brandId = brand.Id;
        }

        [TearDown]
        public void TearDown() => _db.Dispose();

        [Test]
        public async Task GetBrandForManageHandler_ExistingBrand_ReturnsSuccess()
        {
            var handler = new GetBrandForManageHandler(_repo);
            var query = new GetBrandForManageQuery { BrandId = _brandId };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Name, Is.EqualTo("Test Brand"));
        }

        [Test]
        public async Task GetAllBrandsForManageHandler_ReturnsBrands()
        {
            var handler = new GetAllBrandsForManageHandler(_repo);
            var query = new GetAllBrandsForManageQuery { Pagination = new Likano.Application.Common.Models.Pagination { PageNumber = 1, PageSize = 10 } };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Items, Is.Not.Null);
            Assert.That(result.Items!.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task ChangeBrandActiveStatusHandler_ExistingBrand_ReturnsSuccess()
        {
            var handler = new ChangeBrandActiveStatusHandler(_repo);
            var command = new ChangeBrandActiveStatusCommand { BrandId = _brandId };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task CreateBrandForManageHandler_ValidName_ReturnsSuccess()
        {
            var handler = new CreateBrandForManageHandler(_repo, null!);
            var command = new CreateBrandForManageCommand { Name = "New Brand", Description = "Desc" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Id, Is.GreaterThan(0));
        }
    }
}