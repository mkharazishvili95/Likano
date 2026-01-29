using Likano.Application.Features.Manage.Category.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Category.Queries.Get;
using Likano.Application.Features.Manage.Category.Queries.GetAll;
using Likano.Domain.Entities;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Tests.Helpers;
using Microsoft.Extensions.Options;

namespace Likano.Tests.Handlers
{
    [TestFixture]
    public class CategoryHandlersTests
    {
        ApplicationDbContext _db = null!;
        ManageRepository _repo = null!;
        int _categoryId;

        [SetUp]
        public async Task SetUp()
        {
            _db = DbContextHelper.GetInMemoryDbContext();
            var imageKitSettings = Options.Create(new Likano.Application.Configuration.ImageKitSettings());
            _repo = new ManageRepository(_db, imageKitSettings);

            var category = new Category
            {
                Name = "Test Category",
                Description = "Test Description",
                IsActive = true
            };
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            _categoryId = category.Id;
        }

        [TearDown]
        public void TearDown() => _db.Dispose();

        [Test]
        public async Task GetCategoryForManageHandler_ExistingCategory_ReturnsSuccess()
        {
            var handler = new GetCategoryForManageHandler(_repo);
            var query = new GetCategoryForManageQuery { CategoryId = _categoryId };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Name, Is.EqualTo("Test Category"));
        }

        [Test]
        public async Task GetAllCategoriesForManageHandler_ReturnsCategories()
        {
            var handler = new GetAllCategoriesForManageHandler(_repo);
            var query = new GetAllCategoriesForManageQuery { Pagination = new Likano.Application.Common.Models.Pagination { PageNumber = 1, PageSize = 10 } };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Items, Is.Not.Null);
            Assert.That(result.Items!.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task ChangeActiveStatusHandler_ExistingCategory_ReturnsSuccess()
        {
            var handler = new ChangeActiveStatusHandler(_repo);
            var command = new ChangeActiveStatusCommand { CategoryId = _categoryId };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
    }
}