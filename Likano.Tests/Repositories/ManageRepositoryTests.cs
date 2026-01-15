using Likano.Application.Configuration;
using Likano.Domain.Entities;
using Likano.Domain.Enums;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Likano.Tests.Repositories
{
    [TestFixture]
    public class ManageRepositoryTests
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

        #region Product Tests

        [Test]
        public async Task GetProduct_ExistingId_ReturnsProductWithIncludes()
        {
            var result = await _repo.GetProduct(_productId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo("Test Product"));
            Assert.That(result.Category, Is.Not.Null);
            Assert.That(result.Brand, Is.Not.Null);
            Assert.That(result.ProducerCountry, Is.Not.Null);
        }

        [Test]
        public async Task GetProduct_NonExistingId_ReturnsNull()
        {
            var result = await _repo.GetProduct(9999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllProducts_ReturnsAllProducts()
        {
            var result = await _repo.GetAllProducts();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task ChangeStatus_ExistingProduct_ReturnsTrue()
        {
            var result = await _repo.ChangeStatus(_productId, ProductStatus.Deleted);

            Assert.That(result, Is.True);

            var product = await _db.Products.FindAsync(_productId);
            Assert.That(product!.Status, Is.EqualTo(ProductStatus.Deleted));
        }

        [Test]
        public async Task ChangeStatus_NonExistingProduct_ReturnsFalse()
        {
            var result = await _repo.ChangeStatus(9999, ProductStatus.Active);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ChangeCategory_ExistingProduct_ReturnsTrue()
        {
            var newCategory = new Category { Name = "New Category" };
            _db.Categories.Add(newCategory);
            await _db.SaveChangesAsync();

            var result = await _repo.ChangeCategory(_productId, newCategory.Id);

            Assert.That(result, Is.True);

            var product = await _db.Products.FindAsync(_productId);
            Assert.That(product!.CategoryId, Is.EqualTo(newCategory.Id));
        }

        [Test]
        public async Task ChangeCategory_NonExistingProduct_ReturnsFalse()
        {
            var result = await _repo.ChangeCategory(9999, _categoryId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ChangeBrand_ExistingProduct_ReturnsTrue()
        {
            var newBrand = new Brand { Name = "New Brand", IsActive = true };
            _db.Brands.Add(newBrand);
            await _db.SaveChangesAsync();

            var result = await _repo.ChangeBrand(_productId, newBrand.Id);

            Assert.That(result, Is.True);

            var product = await _db.Products.FindAsync(_productId);
            Assert.That(product!.BrandId, Is.EqualTo(newBrand.Id));
        }

        [Test]
        public async Task ChangeBrand_NonExistingProduct_ReturnsFalse()
        {
            var result = await _repo.ChangeBrand(9999, _brandId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ChangeCountry_ExistingProduct_ReturnsTrue()
        {
            var newCountry = new ProducerCountry { Name = "USA" };
            _db.ProducerCountries.Add(newCountry);
            await _db.SaveChangesAsync();

            var result = await _repo.ChangeCountry(_productId, newCountry.Id);

            Assert.That(result, Is.True);

            var product = await _db.Products.FindAsync(_productId);
            Assert.That(product!.ProducerCountryId, Is.EqualTo(newCountry.Id));
        }

        [Test]
        public async Task ChangeCountry_NonExistingCountry_ReturnsFalse()
        {
            var result = await _repo.ChangeCountry(_productId, 9999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddProductAsync_ReturnsProductId()
        {
            var product = new Product
            {
                Title = "New Product",
                CategoryId = _categoryId,
                CreateDate = DateTime.UtcNow
            };

            var result = await _repo.AddProductAsync(product);

            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public async Task UpdateProductAsync_ReturnsTrue()
        {
            var product = await _db.Products.FindAsync(_productId);
            product!.Title = "Updated Product Title";

            var result = await _repo.UpdateProductAsync(product);

            Assert.That(result, Is.True);

            var updated = await _db.Products.FindAsync(_productId);
            Assert.That(updated!.Title, Is.EqualTo("Updated Product Title"));
        }

        [Test]
        public async Task UpdateProductImageUrlAsync_ExistingProduct_ReturnsTrue()
        {
            var result = await _repo.UpdateProductImageUrlAsync(_productId, "https://new-image.jpg");

            Assert.That(result, Is.True);

            var product = await _db.Products.FindAsync(_productId);
            Assert.That(product!.ImageUrl, Is.EqualTo("https://new-image.jpg"));
        }

        [Test]
        public async Task UpdateProductImageUrlAsync_NonExistingProduct_ReturnsFalse()
        {
            var result = await _repo.UpdateProductImageUrlAsync(9999, "https://new-image.jpg");

            Assert.That(result, Is.False);
        }

        #endregion

        #region Category Tests

        [Test]
        public async Task GetCategory_ExistingId_ReturnsCategory()
        {
            var result = await _repo.GetCategory(_categoryId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Test Category"));
        }

        [Test]
        public async Task GetCategory_NonExistingId_ReturnsNull()
        {
            var result = await _repo.GetCategory(9999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllCategories_ReturnsAllCategories()
        {
            var result = await _repo.GetAllCategories();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task ChangeActiveStatusCategory_TogglesStatus()
        {
            var categoryBefore = await _db.Categories.FindAsync(_categoryId);
            var statusBefore = categoryBefore!.IsActive;

            var result = await _repo.ChangeActiveStatusCategory(_categoryId);

            Assert.That(result, Is.True);

            var categoryAfter = await _db.Categories.FindAsync(_categoryId);
            Assert.That(categoryAfter!.IsActive, Is.Not.EqualTo(statusBefore));
        }

        [Test]
        public async Task ChangeActiveStatusCategory_NonExistingCategory_ReturnsFalse()
        {
            var result = await _repo.ChangeActiveStatusCategory(9999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddCategoryAsync_ReturnsCategoryId()
        {
            var category = new Category
            {
                Name = "New Category",
                Description = "New Description"
            };

            var result = await _repo.AddCategoryAsync(category);

            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public async Task EditCategoryAsync_ExistingCategory_ReturnsTrue()
        {
            var result = await _repo.EditCategoryAsync(_categoryId, "Updated Name", "Updated Description");

            Assert.That(result, Is.True);

            var category = await _db.Categories.FindAsync(_categoryId);
            Assert.That(category!.Name, Is.EqualTo("Updated Name"));
            Assert.That(category.Description, Is.EqualTo("Updated Description"));
        }

        [Test]
        public async Task EditCategoryAsync_NonExistingCategory_ReturnsFalse()
        {
            var result = await _repo.EditCategoryAsync(9999, "Name", "Description");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateCategoryLogoAsync_ExistingCategory_ReturnsTrue()
        {
            var result = await _repo.UpdateCategoryLogoAsync(_categoryId, "https://logo.jpg");

            Assert.That(result, Is.True);

            var category = await _db.Categories.FindAsync(_categoryId);
            Assert.That(category!.Logo, Is.EqualTo("https://logo.jpg"));
        }

        [Test]
        public async Task UpdateCategoryLogoAsync_NonExistingCategory_ReturnsFalse()
        {
            var result = await _repo.UpdateCategoryLogoAsync(9999, "https://logo.jpg");

            Assert.That(result, Is.False);
        }

        #endregion

        #region Brand Tests

        [Test]
        public async Task GetBrand_ExistingId_ReturnsBrand()
        {
            var result = await _repo.GetBrand(_brandId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Test Brand"));
        }

        [Test]
        public async Task GetBrand_NonExistingId_ReturnsNull()
        {
            var result = await _repo.GetBrand(9999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllBrands_ReturnsAllBrands()
        {
            var result = await _repo.GetAllBrands();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task ChangeActiveStatusBrand_TogglesStatus()
        {
            var brandBefore = await _db.Brands.FindAsync(_brandId);
            var statusBefore = brandBefore!.IsActive;

            var result = await _repo.ChangeActiveStatusBrand(_brandId);

            Assert.That(result, Is.True);

            var brandAfter = await _db.Brands.FindAsync(_brandId);
            Assert.That(brandAfter!.IsActive, Is.Not.EqualTo(statusBefore));
        }

        [Test]
        public async Task ChangeActiveStatusBrand_NonExistingBrand_ReturnsFalse()
        {
            var result = await _repo.ChangeActiveStatusBrand(9999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddBrandAsync_ReturnsBrandId()
        {
            var brand = new Brand
            {
                Name = "New Brand",
                Description = "New Brand Description",
                IsActive = true
            };

            var result = await _repo.AddBrandAsync(brand);

            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public async Task EditBrandAsync_ExistingBrand_ReturnsTrue()
        {
            var result = await _repo.EditBrandAsync(_brandId, "Updated Brand Name", "Updated Description");

            Assert.That(result, Is.True);

            var brand = await _db.Brands.FindAsync(_brandId);
            Assert.That(brand!.Name, Is.EqualTo("Updated Brand Name"));
            Assert.That(brand.Description, Is.EqualTo("Updated Description"));
        }

        [Test]
        public async Task EditBrandAsync_NonExistingBrand_ReturnsFalse()
        {
            var result = await _repo.EditBrandAsync(9999, "Name", "Description");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateBrandLogoAsync_ExistingBrand_ReturnsTrue()
        {
            var result = await _repo.UpdateBrandLogoAsync(_brandId, "https://brand-logo.jpg");

            Assert.That(result, Is.True);

            var brand = await _db.Brands.FindAsync(_brandId);
            Assert.That(brand!.Logo, Is.EqualTo("https://brand-logo.jpg"));
        }

        [Test]
        public async Task UpdateBrandLogoAsync_NonExistingBrand_ReturnsFalse()
        {
            var result = await _repo.UpdateBrandLogoAsync(9999, "https://logo.jpg");

            Assert.That(result, Is.False);
        }

        #endregion

        #region ProducerCountry Tests

        [Test]
        public async Task GetProducerCountry_ExistingId_ReturnsCountry()
        {
            var result = await _repo.GetProducerCountry(_countryId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Georgia"));
        }

        [Test]
        public async Task GetProducerCountry_NonExistingId_ReturnsNull()
        {
            var result = await _repo.GetProducerCountry(9999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllProducerCountries_ReturnsAllCountries()
        {
            var result = await _repo.GetAllProducerCountries();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task AddProducerCountry_ValidCountry_ReturnsTrue()
        {
            var country = new ProducerCountry { Name = "Germany" };

            var result = await _repo.AddProducerCountry(country);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task AddProducerCountry_NullCountry_ReturnsFalse()
        {
            var result = await _repo.AddProducerCountry(null!);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditProducerCountry_ExistingCountry_ReturnsTrue()
        {
            var result = await _repo.EditProducerCountry(_countryId, "Updated Country Name");

            Assert.That(result, Is.True);

            var country = await _db.ProducerCountries.FindAsync(_countryId);
            Assert.That(country!.Name, Is.EqualTo("Updated Country Name"));
        }

        [Test]
        public async Task EditProducerCountry_NonExistingCountry_ReturnsFalse()
        {
            var result = await _repo.EditProducerCountry(9999, "Name");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteProducerCountry_NonExistingCountry_ReturnsFalse()
        {
            var result = await _repo.DeleteProducerCountry(9999);

            Assert.That(result, Is.False);
        }

        #endregion

        #region File Tests

        [Test]
        public async Task GetFileAsync_ExistingFile_ReturnsFile()
        {
            var result = await _repo.GetFileAsync(_fileId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FileName, Is.EqualTo("test.jpg"));
        }

        [Test]
        public async Task GetFileAsync_DeletedFile_ReturnsNull()
        {
            var file = await _db.Files.FindAsync(_fileId);
            file!.IsDeleted = true;
            await _db.SaveChangesAsync();

            var result = await _repo.GetFileAsync(_fileId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetFileAsync_NonExistingFile_ReturnsNull()
        {
            var result = await _repo.GetFileAsync(9999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteFileAsync_ExistingFile_ReturnsTrue()
        {
            var result = await _repo.DeleteFileAsync(_fileId);

            Assert.That(result, Is.True);

            var file = await _db.Files.FindAsync(_fileId);
            Assert.That(file!.IsDeleted, Is.True);
            Assert.That(file.DeleteDate, Is.Not.Null);
        }

        [Test]
        public async Task DeleteFileAsync_AlreadyDeletedFile_ReturnsFalse()
        {
            var file = await _db.Files.FindAsync(_fileId);
            file!.IsDeleted = true;
            await _db.SaveChangesAsync();

            var result = await _repo.DeleteFileAsync(_fileId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteFileAsync_NonExistingFile_ReturnsFalse()
        {
            var result = await _repo.DeleteFileAsync(9999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetAllFiles_ReturnsNonDeletedFilesOnly()
        {
            var deletedFile = new Likano.Domain.Entities.File
            {
                FileName = "deleted.jpg",
                IsDeleted = true
            };
            _db.Files.Add(deletedFile);
            await _db.SaveChangesAsync();

            var result = await _repo.GetAllFiles();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.All(f => !f.IsDeleted), Is.True);
        }

        [Test]
        public async Task SetMainImageAsync_SetsCorrectMainImage()
        {
            var secondFile = new Likano.Domain.Entities.File
            {
                FileName = "second.jpg",
                ProductId = _productId,
                IsDeleted = false,
                MainImage = false
            };
            _db.Files.Add(secondFile);
            await _db.SaveChangesAsync();

            var result = await _repo.SetMainImageAsync(_productId, secondFile.Id);

            Assert.That(result, Is.True);

            var files = await _db.Files.Where(f => f.ProductId == _productId && !f.IsDeleted).ToListAsync();
            Assert.That(files.Single(f => f.Id == secondFile.Id).MainImage, Is.True);
            Assert.That(files.Single(f => f.Id == _fileId).MainImage, Is.False);
        }

        [Test]
        public async Task GetMainImageForProductAsync_ReturnsMainImage()
        {
            var file = await _db.Files.FindAsync(_fileId);
            file!.MainImage = true;
            await _db.SaveChangesAsync();

            var result = await _repo.GetMainImageForProductAsync(_productId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.MainImage, Is.True);
        }

        [Test]
        public async Task GetProductImagesAsync_ReturnsOrderedImages()
        {
            var mainFile = await _db.Files.FindAsync(_fileId);
            mainFile!.MainImage = true;
            await _db.SaveChangesAsync();

            var secondFile = new Likano.Domain.Entities.File
            {
                FileName = "second.jpg",
                ProductId = _productId,
                IsDeleted = false,
                MainImage = false
            };
            _db.Files.Add(secondFile);
            await _db.SaveChangesAsync();

            var result = await _repo.GetProductImagesAsync(_productId);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First().MainImage, Is.True);
        }

        #endregion

        #region DeleteImage Tests

        [Test]
        public async Task DeleteImage_ForCategory_NullifiesLogo()
        {
            var category = await _db.Categories.FindAsync(_categoryId);
            category!.Logo = "https://logo.jpg";
            await _db.SaveChangesAsync();

            var result = await _repo.DeleteImage(_categoryId, null, null);

            Assert.That(result, Is.True);

            var updated = await _db.Categories.FindAsync(_categoryId);
            Assert.That(updated!.Logo, Is.Null);
        }

        [Test]
        public async Task DeleteImage_ForBrand_NullifiesLogo()
        {
            var brand = await _db.Brands.FindAsync(_brandId);
            brand!.Logo = "https://brand-logo.jpg";
            await _db.SaveChangesAsync();

            var result = await _repo.DeleteImage(null, _brandId, null);

            Assert.That(result, Is.True);

            var updated = await _db.Brands.FindAsync(_brandId);
            Assert.That(updated!.Logo, Is.Null);
        }

        [Test]
        public async Task DeleteImage_ForProduct_NullifiesImageUrl()
        {
            var product = await _db.Products.FindAsync(_productId);
            product!.ImageUrl = "https://product-image.jpg";
            await _db.SaveChangesAsync();

            var result = await _repo.DeleteImage(null, null, _productId);

            Assert.That(result, Is.True);

            var updated = await _db.Products.FindAsync(_productId);
            Assert.That(updated!.ImageUrl, Is.Null);
        }

        [Test]
        public async Task DeleteImage_NoParameters_ReturnsFalse()
        {
            var result = await _repo.DeleteImage(null, null, null);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteImage_NonExistingCategory_ReturnsFalse()
        {
            var result = await _repo.DeleteImage(9999, null, null);

            Assert.That(result, Is.False);
        }

        #endregion

        #region Categories Search Tests

        [Test]
        public async Task Categories_WithSearchString_ReturnsFilteredCategories()
        {
            var result = await _repo.Categories("Test", null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.All(c => c.Name.Contains("Test")), Is.True);
        }

        [Test]
        public async Task Categories_WithId_ReturnsCategoryById()
        {
            var result = await _repo.Categories(null, _categoryId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(_categoryId));
        }

        [Test]
        public async Task Categories_NoMatch_ReturnsNull()
        {
            var result = await _repo.Categories("NonExistingCategory12345", null);

            Assert.That(result, Is.Null);
        }

        #endregion
    }
}