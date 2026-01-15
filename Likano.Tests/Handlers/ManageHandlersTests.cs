using Likano.Application.Common.Models;
using Likano.Application.Configuration;
using Likano.Application.Features.Manage.Brand.Commands.Change;
using Likano.Application.Features.Manage.Brand.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Brand.Commands.Create;
using Likano.Application.Features.Manage.Brand.Queries.Get;
using Likano.Application.Features.Manage.Brand.Queries.GetAll;
using Likano.Application.Features.Manage.Category.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Category.Queries.Get;
using Likano.Application.Features.Manage.Category.Queries.GetAll;
using Likano.Application.Features.Manage.File.Commands.Delete;
using Likano.Application.Features.Manage.File.Queries.Get;
using Likano.Application.Features.Manage.ProducerCountry.Commands.Change;
using Likano.Application.Features.Manage.ProducerCountry.Commands.Create;
using Likano.Application.Features.Manage.ProducerCountry.Commands.Delete;
using Likano.Application.Features.Manage.ProducerCountry.Commands.Edit;
using Likano.Application.Features.Manage.ProducerCountry.Queries.GetAll;
using Likano.Application.Features.Manage.Product.Commands.ChangeCategory;
using Likano.Application.Features.Manage.Product.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Product.Commands.Create;
using Likano.Application.Features.Manage.Product.Commands.Edit;
using Likano.Application.Features.Manage.Product.Queries.Get;
using Likano.Application.Features.Manage.Product.Queries.GetAll;
using Likano.Domain.Entities;
using Likano.Domain.Enums;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Tests.Helpers;
using Microsoft.Extensions.Options;

namespace Likano.Tests.Handlers
{
    [TestFixture]
    public class ManageHandlersTests
    {
        ApplicationDbContext _db = null!;
        ManageRepository _repo = null!;
        int _productId;
        int _categoryId;
        int _brandId;
        int _countryId;
        int _fileId;

        [SetUp]
        public async Task SetUp()
        {
            _db = DbContextHelper.GetInMemoryDbContext();

            var imageKitSettings = Options.Create(new ImageKitSettings
            {
                PublicKey = "test_public_key",
                PrivateKey = "test_private_key",
                UrlEndpoint = "https://test.imagekit.io"
            });

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

            var brand = new Brand
            {
                Name = "Test Brand",
                Description = "Test Brand Description",
                IsActive = true
            };
            _db.Brands.Add(brand);
            await _db.SaveChangesAsync();
            _brandId = brand.Id;

            var country = new ProducerCountry
            {
                Name = "Georgia"
            };
            _db.ProducerCountries.Add(country);
            await _db.SaveChangesAsync();
            _countryId = country.Id;

            var product = new Product
            {
                Title = "Test Product",
                Description = "Test Product Description",
                Price = 100,
                IsAvailable = true,
                CategoryId = _categoryId,
                BrandId = _brandId,
                ProducerCountryId = _countryId,
                Status = ProductStatus.Active,
                CreateDate = DateTime.UtcNow
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            _productId = product.Id;

            var file = new Likano.Domain.Entities.File
            {
                FileName = "test.jpg",
                FileUrl = "https://test.imagekit.io/test.jpg",
                FileType = Domain.Enums.File.FileType.Image,
                UploadDate = DateTime.UtcNow,
                IsDeleted = false,
                ProductId = _productId
            };
            _db.Files.Add(file);
            await _db.SaveChangesAsync();
            _fileId = file.Id;
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        #region Category Handlers

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
        public async Task GetCategoryForManageHandler_NonExistingCategory_Returns404()
        {
            var handler = new GetCategoryForManageHandler(_repo);
            var query = new GetCategoryForManageQuery { CategoryId = 9999 };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetAllCategoriesForManageHandler_ReturnsCategories()
        {
            var handler = new GetAllCategoriesForManageHandler(_repo);
            var query = new GetAllCategoriesForManageQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 10 }
            };

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

        [Test]
        public async Task ChangeActiveStatusHandler_NonExistingCategory_Returns404()
        {
            var handler = new ChangeActiveStatusHandler(_repo);
            var command = new ChangeActiveStatusCommand { CategoryId = 9999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        #endregion

        #region Brand Handlers

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
        public async Task GetBrandForManageHandler_NonExistingBrand_Returns404()
        {
            var handler = new GetBrandForManageHandler(_repo);
            var query = new GetBrandForManageQuery { BrandId = 9999 };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetAllBrandsForManageHandler_ReturnsBrands()
        {
            var handler = new GetAllBrandsForManageHandler(_repo);
            var query = new GetAllBrandsForManageQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 10 }
            };

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
        public async Task ChangeBrandActiveStatusHandler_NonExistingBrand_Returns404()
        {
            var handler = new ChangeBrandActiveStatusHandler(_repo);
            var command = new ChangeBrandActiveStatusCommand { BrandId = 9999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task CreateBrandForManageHandler_EmptyName_Returns400()
        {
            var handler = new CreateBrandForManageHandler(_repo, null!);
            var command = new CreateBrandForManageCommand { Name = "" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task CreateBrandForManageHandler_ValidName_ReturnsSuccess()
        {
            var handler = new CreateBrandForManageHandler(_repo, null!);
            var command = new CreateBrandForManageCommand
            {
                Name = "New Brand",
                Description = "New Brand Description"
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Id, Is.GreaterThan(0));
        }

        [Test]
        public async Task ChangeBrandHandler_NonExistingProduct_Returns404()
        {
            var handler = new ChangeBrandHandler(_repo);
            var command = new ChangeBrandCommand { ProductId = 9999, NewBrandId = _brandId };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task ChangeBrandHandler_NonExistingBrand_Returns404()
        {
            var handler = new ChangeBrandHandler(_repo);
            var command = new ChangeBrandCommand { ProductId = _productId, NewBrandId = 9999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task ChangeBrandHandler_SameBrand_Returns400()
        {
            var handler = new ChangeBrandHandler(_repo);
            var command = new ChangeBrandCommand { ProductId = _productId, NewBrandId = _brandId };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task ChangeBrandHandler_ValidChange_ReturnsSuccess()
        {
            var newBrand = new Brand { Name = "New Brand", IsActive = true };
            _db.Brands.Add(newBrand);
            await _db.SaveChangesAsync();

            var handler = new ChangeBrandHandler(_repo);
            var command = new ChangeBrandCommand { ProductId = _productId, NewBrandId = newBrand.Id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        #endregion

        #region Product Handlers

        [Test]
        public async Task GetProductForManageHandler_ExistingProduct_ReturnsSuccess()
        {
            var handler = new GetProductForManageHandler(_repo);
            var query = new GetProductForManageQuery { Id = _productId };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Title, Is.EqualTo("Test Product"));
        }

        [Test]
        public async Task GetProductForManageHandler_NonExistingProduct_Returns404()
        {
            var handler = new GetProductForManageHandler(_repo);
            var query = new GetProductForManageQuery { Id = 9999 };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetAllProductsForManageHandler_ReturnsProducts()
        {
            var handler = new GetAllProductsForManageHandler(_repo);
            var query = new GetAllProductsForManageQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 10 }
            };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Items, Is.Not.Null);
            Assert.That(result.Items!.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task CreateProductForManageHandler_EmptyTitle_Returns400()
        {
            var handler = new CreateProductForManageHandler(_repo);
            var command = new CreateProductForManageCommand { Title = "" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task CreateProductForManageHandler_ValidData_ReturnsSuccess()
        {
            var handler = new CreateProductForManageHandler(_repo);
            var command = new CreateProductForManageCommand
            {
                Title = "New Product",
                Description = "Description",
                Price = 50,
                CategoryId = _categoryId
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.ProductId, Is.GreaterThan(0));
        }

        [Test]
        public async Task EditProductForManageHandler_EmptyTitle_Returns400()
        {
            var handler = new EditProductForManageHandler(_repo);
            var command = new EditProductForManageCommand { Id = _productId, Title = "" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task EditProductForManageHandler_NonExistingProduct_Returns404()
        {
            var handler = new EditProductForManageHandler(_repo);
            var command = new EditProductForManageCommand { Id = 9999, Title = "Title" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task EditProductForManageHandler_ValidData_ReturnsSuccess()
        {
            var handler = new EditProductForManageHandler(_repo);
            var command = new EditProductForManageCommand
            {
                Id = _productId,
                Title = "Updated Title",
                CategoryId = _categoryId
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task ChangeProductStatusHandler_NonExistingProduct_Returns404()
        {
            var handler = new ChangeProductStatusHandler(_repo);
            var command = new ChangeProductStatusCommand { ProductId = 9999, Status = ProductStatus.Deleted };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task ChangeProductStatusHandler_SameStatus_Returns400()
        {
            var handler = new ChangeProductStatusHandler(_repo);
            var command = new ChangeProductStatusCommand { ProductId = _productId, Status = ProductStatus.Active };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task ChangeProductStatusHandler_ValidChange_ReturnsSuccess()
        {
            var handler = new ChangeProductStatusHandler(_repo);
            var command = new ChangeProductStatusCommand { ProductId = _productId, Status = ProductStatus.Deleted };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task ChangeCategoryHandler_NonExistingProduct_Returns404()
        {
            var handler = new ChangeCategoryHandler(_repo);
            var command = new ChangeCategoryCommand { ProductId = 9999, NewCategoryId = _categoryId };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task ChangeCategoryHandler_NonExistingCategory_Returns404()
        {
            var handler = new ChangeCategoryHandler(_repo);
            var command = new ChangeCategoryCommand { ProductId = _productId, NewCategoryId = 9999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task ChangeCategoryHandler_SameCategory_Returns400()
        {
            var handler = new ChangeCategoryHandler(_repo);
            var command = new ChangeCategoryCommand { ProductId = _productId, NewCategoryId = _categoryId };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task ChangeCategoryHandler_ValidChange_ReturnsSuccess()
        {
            var newCategory = new Category { Name = "New Category", IsActive = true };
            _db.Categories.Add(newCategory);
            await _db.SaveChangesAsync();

            var handler = new ChangeCategoryHandler(_repo);
            var command = new ChangeCategoryCommand { ProductId = _productId, NewCategoryId = newCategory.Id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        #endregion

        #region ProducerCountry Handlers

        [Test]
        public async Task GetAllProducerCountriesForManageHandler_ReturnsCountries()
        {
            var handler = new GetAllProducerCountriesForManageHandler(_repo);
            var query = new GetAllProducerCountriesForManageQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 10 }
            };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Items, Is.Not.Null);
            Assert.That(result.Items!.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task GetAllProducerCountriesForManageHandler_FilterById_ReturnsFiltered()
        {
            var handler = new GetAllProducerCountriesForManageHandler(_repo);
            var query = new GetAllProducerCountriesForManageQuery { Id = _countryId };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Items!.Count, Is.EqualTo(1));
            Assert.That(result.Items[0].Id, Is.EqualTo(_countryId));
        }

        [Test]
        public async Task CreateProducerCountryForManageHandler_EmptyName_Returns400()
        {
            var handler = new CreateProducerCountryForManageHandler(_repo);
            var command = new CreateProducerCountryForManageCommand { Name = "" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task CreateProducerCountryForManageHandler_ValidName_ReturnsSuccess()
        {
            var handler = new CreateProducerCountryForManageHandler(_repo);
            var command = new CreateProducerCountryForManageCommand { Name = "Germany" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(201));
            Assert.That(result.Id, Is.GreaterThan(0));
        }

        [Test]
        public async Task EditProducerCountryForManageHandler_InvalidId_Returns400()
        {
            var handler = new EditProducerCountryForManageHandler(_repo);
            var command = new EditProducerCountryForManageCommand { Id = 0, Name = "Test" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task EditProducerCountryForManageHandler_EmptyName_Returns400()
        {
            var handler = new EditProducerCountryForManageHandler(_repo);
            var command = new EditProducerCountryForManageCommand { Id = _countryId, Name = "" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task EditProducerCountryForManageHandler_ValidData_ReturnsSuccess()
        {
            var handler = new EditProducerCountryForManageHandler(_repo);
            var command = new EditProducerCountryForManageCommand { Id = _countryId, Name = "Updated Country" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task EditProducerCountryForManageHandler_NonExistingCountry_Returns404()
        {
            var handler = new EditProducerCountryForManageHandler(_repo);
            var command = new EditProducerCountryForManageCommand { Id = 9999, Name = "Test" };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task DeleteProducerCountryForManageHandler_InvalidId_Returns400()
        {
            var handler = new DeleteProducerCountryForManageHandler(_repo);
            var command = new DeleteProducerCountryForManageCommand { Id = 0 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task DeleteProducerCountryForManageHandler_NonExistingCountry_Returns404()
        {
            var handler = new DeleteProducerCountryForManageHandler(_repo);
            var command = new DeleteProducerCountryForManageCommand { Id = 9999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task ChangeCountryHandler_NonExistingProduct_Returns404()
        {
            var handler = new ChangeCountryHandler(_repo);
            var command = new ChangeCountryCommand { ProductId = 9999, NewCountryId = _countryId };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task ChangeCountryHandler_NonExistingCountry_Returns404()
        {
            var handler = new ChangeCountryHandler(_repo);
            var command = new ChangeCountryCommand { ProductId = _productId, NewCountryId = 9999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task ChangeCountryHandler_ValidChange_ReturnsSuccess()
        {
            var newCountry = new ProducerCountry { Name = "USA" };
            _db.ProducerCountries.Add(newCountry);
            await _db.SaveChangesAsync();

            var handler = new ChangeCountryHandler(_repo);
            var command = new ChangeCountryCommand { ProductId = _productId, NewCountryId = newCountry.Id };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        #endregion

        #region File Handlers

        [Test]
        public async Task GetFileForManageHandler_ExistingFile_ReturnsSuccess()
        {
            var handler = new GetFileForManageHandler(_repo);
            var query = new GetFileForManageQuery { FileId = _fileId };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.FileName, Is.EqualTo("test.jpg"));
        }

        [Test]
        public async Task GetFileForManageHandler_NonExistingFile_Returns404()
        {
            var handler = new GetFileForManageHandler(_repo);
            var query = new GetFileForManageQuery { FileId = 9999 };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task DeleteFileForManageHandler_ExistingFile_ReturnsSuccess()
        {
            var handler = new DeleteFileForManageHandler(_repo);
            var command = new DeleteFileForManageCommand { FileId = _fileId };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task DeleteFileForManageHandler_NonExistingFile_Returns404()
        {
            var handler = new DeleteFileForManageHandler(_repo);
            var command = new DeleteFileForManageCommand { FileId = 9999 };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task DeleteFileForManageHandler_AlreadyDeletedFile_Returns404()
        {
            var file = await _db.Files.FindAsync(_fileId);
            file!.IsDeleted = true;
            await _db.SaveChangesAsync();

            var handler = new DeleteFileForManageHandler(_repo);
            var command = new DeleteFileForManageCommand { FileId = _fileId };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        #endregion
    }
}