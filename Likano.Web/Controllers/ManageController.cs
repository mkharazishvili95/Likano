using Likano.Application.Common.Models;
using Likano.Application.DTOs;
using Likano.Application.Features.Category.Queries.GetAll;
using Likano.Application.Features.Manage.Brand.Commands.Change;
using Likano.Application.Features.Manage.Brand.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Brand.Commands.Create;
using Likano.Application.Features.Manage.Brand.Commands.Edit;
using Likano.Application.Features.Manage.Brand.Queries.Get;
using Likano.Application.Features.Manage.Brand.Queries.GetAll;
using Likano.Application.Features.Manage.Category.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Category.Commands.Create;
using Likano.Application.Features.Manage.Category.Commands.Edit;
using Likano.Application.Features.Manage.Category.Queries.Get;
using Likano.Application.Features.Manage.Category.Queries.GetAll;
using Likano.Application.Features.Manage.ProducerCountry.Commands.Change;
using Likano.Application.Features.Manage.ProducerCountry.Commands.Create;
using Likano.Application.Features.Manage.ProducerCountry.Commands.Delete;
using Likano.Application.Features.Manage.ProducerCountry.Commands.Edit;
using Likano.Application.Features.Manage.ProducerCountry.Queries.Get;
using Likano.Application.Features.Manage.ProducerCountry.Queries.GetAll;
using Likano.Application.Features.Manage.Product.Commands.ChangeAvailableStatus;
using Likano.Application.Features.Manage.Product.Commands.ChangeCategory;
using Likano.Application.Features.Manage.Product.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Product.Commands.ChangeType;
using Likano.Application.Features.Manage.Product.Commands.Create;
using Likano.Application.Features.Manage.Product.Commands.Edit;
using Likano.Application.Features.Manage.Product.Queries.Get;
using Likano.Application.Features.Manage.Product.Queries.GetAll;
using Likano.Domain.Entities;
using Likano.Domain.Enums;
using Likano.Domain.Enums.User;
using Likano.Web.Models.Manage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Likano.Web.Controllers
{
    [Authorize(Roles = nameof(UserType.Admin))]
    public class ManageController : Controller
    {
        readonly HttpClient _httpClient;
        readonly string _baseUrl;

        public ManageController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _baseUrl = configuration["ApiSettings:BaseUrl"]!;
        }

        [HttpGet]
        public IActionResult Main()
        {
            return View("Main");
        }

        [HttpGet]
        public async Task<IActionResult> Products([FromQuery] ProductsFilterVm filter)
        {
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var request = new GetAllProductsForManageQuery
            {
                Pagination = new Pagination { PageNumber = pageNumber, PageSize = pageSize },
                Id = filter.Id,
                Title = string.IsNullOrWhiteSpace(filter.Title) ? null : filter.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(filter.Description) ? null : filter.Description.Trim(),
                PriceFrom = filter.PriceFrom,
                PriceTo = filter.PriceTo,
                IsAvailable = filter.IsAvailable,
                ImageUrl = string.IsNullOrWhiteSpace(filter.ImageUrl) ? null : filter.ImageUrl.Trim(),
                CategoryId = filter.CategoryId,
                Status = filter.Status,
                CreateDateFrom = filter.CreateDateFrom,
                CreateDateTo = filter.CreateDateTo,
                UpdateDateFrom = filter.UpdateDateFrom,
                UpdateDateTo = filter.UpdateDateTo,
                BrandId = filter.BrandId,
                Material = string.IsNullOrWhiteSpace(filter.Material) ? null : filter.Material.Trim(),
                LengthFrom = filter.LengthFrom,
                LengthTo = filter.LengthTo,
                WidthFrom = filter.WidthFrom,
                WidthTo = filter.WidthTo,
                HeightFrom = filter.HeightFrom,
                HeightTo = filter.HeightTo,
                Color = string.IsNullOrWhiteSpace(filter.Color) ? null : filter.Color.Trim(),
                ProducerCountryId = filter.ProducerCountryId,
                Code = string.IsNullOrWhiteSpace(filter.Code) ? null : filter.Code.Trim(),
                Type = filter.Type
            };

            var apiUrl = $"{_baseUrl}/manage/products";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, request);
            GetAllProductsForManageResponse data;
            if (!apiResponse.IsSuccessStatusCode)
            {
                data = new GetAllProductsForManageResponse
                {
                    Success = false,
                    StatusCode = (int)apiResponse.StatusCode,
                    Message = $"API error: {(int)apiResponse.StatusCode}",
                    TotalCount = 0,
                    Items = new List<GetAllProductsForManageItemsResponse>()
                };
            }
            else
            {
                data = await apiResponse.Content.ReadFromJsonAsync<GetAllProductsForManageResponse>()
                       ?? new GetAllProductsForManageResponse
                       {
                           Success = false,
                           StatusCode = 500,
                           Message = "Invalid API response",
                           TotalCount = 0,
                           Items = new List<GetAllProductsForManageItemsResponse>()
                       };
            }

            var categoriesUrl = $"{_baseUrl}/manage/categories";
            var categoriesRequest = new GetAllCategoriesForManageQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 1000 },
                SearchString = null
            };
            var categoriesResponse = await _httpClient.PostAsJsonAsync(categoriesUrl, categoriesRequest);

            var categories = new List<CategoryDtoForManage>();
            if (categoriesResponse.IsSuccessStatusCode)
            {
                var categoriesData = await categoriesResponse.Content.ReadFromJsonAsync<GetAllCategoriesForManageResponse>();
                if (categoriesData?.Items != null)
                {
                    categories = categoriesData.Items
                        .Select(c => new CategoryDtoForManage { Id = (int)c.Id, Name = c.Name })
                        .ToList();
                }
            }

            var brandsUrl = $"{_baseUrl}/manage/brands";
            var brandsRequest = new GetAllBrandsForManageQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 1000 },
                Name = null
            };
            var brandsResponse = await _httpClient.PostAsJsonAsync(brandsUrl, brandsRequest);

            var brands = new List<BrandDtoForManage>();
            if (brandsResponse.IsSuccessStatusCode)
            {
                var brandsData = await brandsResponse.Content.ReadFromJsonAsync<GetAllBrandsForManageResponse>();
                if (brandsData?.Items != null)
                {
                    brands = brandsData.Items
                        .Select(b => new BrandDtoForManage { Id = (int)b.Id, Name = b.Name })
                        .ToList();
                }
            }

            var countriesUrl = $"{_baseUrl}/manage/producer-countries";
            var countriesRequest = new GetAllProducerCountriesForManageQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 1000 },
                Name = null
            };
            var countriesResponse = await _httpClient.PostAsJsonAsync(countriesUrl, countriesRequest);

            var countries = new List<ProducerCountryDtoForManage>();
            if (countriesResponse.IsSuccessStatusCode)
            {
                var countryData = await countriesResponse.Content.ReadFromJsonAsync<GetAllProducerCountriesForManageResponse>();
                if (countryData?.Items != null)
                {
                    countries = countryData.Items
                        .Select(b => new ProducerCountryDtoForManage { Id = (int)b.Id, Name = b.Name })
                        .ToList();
                }
            }

            filter.PageNumber = pageNumber;
            filter.PageSize = pageSize;

            var vm = new ProductsManageViewModel
            {
                Filter = filter,
                Response = data,
                Categories = categories,
                Brands = brands,
                ProducerCountries = countries
            };

            return View("Products", vm);
        }

        //for Product:
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var apiUrl = $"{_baseUrl}/manage/product/{id}";
            var apiResponse = await _httpClient.GetAsync(apiUrl);
            if (!apiResponse.IsSuccessStatusCode)
                return View("NotFound");

            var product = await apiResponse.Content.ReadFromJsonAsync<GetProductForManageResponse>();
            if (product == null || product.Success == false)
                return View("NotFound");

            var categories = await LoadCategories();
            var brands = await LoadBrands();
            var countries = await LoadCountries();

            var vm = new ProductDetailsViewModel
            {
                Product = product,
                Categories = categories,
                Brands = brands,
                Countries = countries
            };

            return View("Details", vm);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProductCountry(int productId, int newCountryId)
        {
            var apiUrl = $"{_baseUrl}/manage/product/change-country";
            var command = new ChangeCountryCommand
            {
                ProductId = productId,
                NewCountryId = newCountryId
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, command);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "ქვეყნის ცვლილება ვერ მოხერხდა";
                return RedirectToAction("Details", new { id = productId });
            }

            var result = await response.Content.ReadFromJsonAsync<ChangeCountryResponse>();
            TempData[(bool)result!.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

            return RedirectToAction("Details", new { id = productId });
        }

        private async Task<List<CategoryDtoForManage>> LoadCategories()
        {
            var categoriesUrl = $"{_baseUrl}/manage/categories";
            var categoriesRequest = new GetAllCategoriesForManageQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 1000 },
                IsActive = true
            };
            var response = await _httpClient.PostAsJsonAsync(categoriesUrl, categoriesRequest);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<GetAllCategoriesForManageResponse>();
                if (data?.Items != null)
                    return data.Items.Select(c => new CategoryDtoForManage { Id = (int)c.Id, Name = c.Name }).ToList();
            }
            return new();
        }

        private async Task<List<BrandDtoForManage>> LoadBrands()
        {
            var brandsUrl = $"{_baseUrl}/manage/brands";
            var brandsRequest = new GetAllBrandsForManageQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 1000 },
                IsActive = true
            };
            var response = await _httpClient.PostAsJsonAsync(brandsUrl, brandsRequest);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<GetAllBrandsForManageResponse>();
                if (data?.Items != null)
                    return data.Items.Select(b => new BrandDtoForManage { Id = (int)b.Id, Name = b.Name }).ToList();
            }
            return new();
        }

        private async Task<List<ProducerCountryDtoForManage>> LoadCountries()
        {
            var countriesUrl = $"{_baseUrl}/manage/producer-countries";
            var countriesRequest = new GetAllProducerCountriesForManageQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 1000 }
            };
            var response = await _httpClient.PostAsJsonAsync(countriesUrl, countriesRequest);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<GetAllProducerCountriesForManageResponse>();
                if (data?.Items != null)
                    return data.Items.Select(c => new ProducerCountryDtoForManage { Id = (int)c.Id, Name = c.Name }).ToList();
            }
            return new();
        }

        public async Task<IActionResult> ChangeProductStatus(int productId, ProductStatus status, bool? intoGrid)
        {
            var apiUrl = $"{_baseUrl}/manage/product/status";

            var command = new ChangeProductStatusCommand
            {
                ProductId = productId,
                Status = status
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, command);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "პროდუქტის სტატუსის ცვლილება ვერ მოხერხდა";
                return intoGrid == true ? RedirectToAction("Products") : RedirectToAction("Details", new { id = productId });
            }

            var result = await response.Content.ReadFromJsonAsync<ChangeProductStatusResponse>();
            TempData[(bool)result!.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

            return intoGrid == true ? RedirectToAction("Products") : RedirectToAction("Details", new { id = productId });
        }

        [HttpGet]
        public async Task<IActionResult> Categories([FromQuery] CategoriesFilterVm filter)
        {
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var request = new GetAllCategoriesForManageQuery
            {
                Pagination = new Pagination { PageNumber = pageNumber, PageSize = pageSize },
                Id = filter.Id,
                Description = filter.Description,
                IsActive = filter.IsActive,
                Name = filter.Name
            };

            var apiUrl = $"{_baseUrl}/manage/categories";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, request);
            GetAllCategoriesForManageResponse data;
            if (!apiResponse.IsSuccessStatusCode)
            {
                data = new GetAllCategoriesForManageResponse
                {
                    Success = false,
                    StatusCode = (int)apiResponse.StatusCode,
                    Message = $"API error: {(int)apiResponse.StatusCode}",
                    TotalCount = 0,
                    Items = new List<GetAllCategoriesForManageItemsResponse>()
                };
            }
            else
            {
                data = await apiResponse.Content.ReadFromJsonAsync<GetAllCategoriesForManageResponse>()
                       ?? new GetAllCategoriesForManageResponse
                       {
                           Success = false,
                           StatusCode = 500,
                           Message = "Invalid API response",
                           TotalCount = 0,
                           Items = new List<GetAllCategoriesForManageItemsResponse>()
                       };
            }

            filter.PageNumber = pageNumber;
            filter.PageSize = pageSize;

            var vm = new CategoriesManageViewModel
            {
                Filter = filter,
                Response = data
            };

            return View("Categories", vm);
        }

        [HttpGet]
        public async Task<IActionResult> CategoryDetails(int id)
        {
            if (id <= 0)
                return NotFound();

            var apiUrl = $"{_baseUrl}/manage/category/{id}";
            var apiResponse = await _httpClient.GetAsync(apiUrl);

            if (!apiResponse.IsSuccessStatusCode)
            {
                return View("NotFoundCategory", new GetCategoryForManageResponse
                {
                    Success = false,
                    Message = $"API error: {(int)apiResponse.StatusCode}"
                });
            }
            var category = await apiResponse.Content.ReadFromJsonAsync<GetCategoryForManageResponse>();
            if (category == null || category.Success == false)
            {
                return View("NotFoundCategory", category ?? new GetCategoryForManageResponse
                {
                    Success = false,
                    Message = "Category not found"
                });
            }

            return View("CategoryDetails", category);
        }

        public async Task<IActionResult> ChangeCategoryStatus(int categoryId, bool? intoGrid)
        {
            var apiUrl = $"{_baseUrl}/manage/category/status";

            var command = new ChangeActiveStatusCommand
            {
                CategoryId = categoryId
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, command);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "კატეგორიის სტატუსის ცვლილება ვერ მოხერხდა";
                return RedirectToAction("Categories");
            }

            var result = await response.Content.ReadFromJsonAsync<ChangeActiveStatusResponse>();
            TempData[(bool)result!.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

            return intoGrid == true ? RedirectToAction("Categories") : RedirectToAction("CategoryDetails", new { id = categoryId });
        }

        public async Task<IActionResult> ChangeProductCategory(int productId, int newCategoryId, int categoryId)
        {
            var apiUrl = $"{_baseUrl}/manage/product/change-category";

            var command = new ChangeCategoryCommand
            {
                ProductId = productId,
                NewCategoryId = newCategoryId
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, command);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "კატეგორიის ცვლილება ვერ მოხერხდა";
                return RedirectToAction("Details", new { id = productId });
            }

            var result = await response.Content.ReadFromJsonAsync<ChangeCategoryResponse>();
            TempData[(bool)result!.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

            return RedirectToAction("Details", new { id = productId });
        }

        public async Task<IActionResult> ChangeProductBrand(int productId, int newBrandId, int brandId)
        {
            var apiUrl = $"{_baseUrl}/manage/product/change-brand";

            var command = new ChangeBrandCommand
            {
                ProductId = productId,
                NewBrandId = newBrandId
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, command);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "ბრენდის ცვლილება ვერ მოხდა";
                return RedirectToAction("Details", new { id = productId });
            }

            var result = await response.Content.ReadFromJsonAsync<ChangeBrandResponse>();
            TempData[(bool)result!.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

            return RedirectToAction("Details", new { id = productId });
        }

        [HttpGet]
        public async Task<IActionResult> Brands([FromQuery] BrandsFilterVm filter)
        {
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var request = new GetAllBrandsForManageQuery
            {
                Pagination = new Pagination { PageNumber = pageNumber, PageSize = pageSize },
                Id = filter.Id,
                Description = filter.Description, 
                IsActive = filter.IsActive,
                Name = filter.Name
            };

            var apiUrl = $"{_baseUrl}/manage/brands";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, request);
            GetAllBrandsForManageResponse data;
            if (!apiResponse.IsSuccessStatusCode)
            {
                data = new GetAllBrandsForManageResponse
                {
                    Success = false,
                    StatusCode = (int)apiResponse.StatusCode,
                    Message = $"API error: {(int)apiResponse.StatusCode}",
                    TotalCount = 0,
                    Items = new List<GetAllBrandsForManageItemsResponse>()
                };
            }
            else
            {
                data = await apiResponse.Content.ReadFromJsonAsync<GetAllBrandsForManageResponse>()
                       ?? new GetAllBrandsForManageResponse
                       {
                           Success = false,
                           StatusCode = 500,
                           Message = "Invalid API response",
                           TotalCount = 0,
                           Items = new List<GetAllBrandsForManageItemsResponse>()
                       };
            }

            filter.PageNumber = pageNumber;
            filter.PageSize = pageSize;

            var vm = new BrandsManageViewModel
            {
                Filter = filter,
                Response = data
            };

            return View("Brands", vm);
        }

        [HttpGet]
        public async Task<IActionResult> BrandDetails(int id)
        {
            if (id <= 0)
                return NotFound();

            var apiUrl = $"{_baseUrl}/manage/brand/{id}";
            var apiResponse = await _httpClient.GetAsync(apiUrl);

            if (!apiResponse.IsSuccessStatusCode)
            {
                return View("NotFoundBrand", new GetBrandForManageResponse
                {
                    Success = false,
                    Message = $"API error: {(int)apiResponse.StatusCode}"
                });
            }
            var brand = await apiResponse.Content.ReadFromJsonAsync<GetBrandForManageResponse>();
            if (brand == null || brand.Success == false)
            {
                return View("NotFoundBrand", brand ?? new GetBrandForManageResponse
                {
                    Success = false,
                    Message = "Brand not found"
                });
            }

            return View("BrandDetails", brand);
        }

        public async Task<IActionResult> ChangeBrandStatus(int brandId, bool? intoGrid)
        {
            var apiUrl = $"{_baseUrl}/manage/brand/status";

            var command = new ChangeBrandActiveStatusCommand
            {
                BrandId = brandId
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, command);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "ბრენდის სტატუსის ცვლილება ვერ მოხერხდა";
                return RedirectToAction("Brands");
            }

            var result = await response.Content.ReadFromJsonAsync<ChangeBrandActiveStatusResponse>();
            TempData[(bool)result!.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

            return intoGrid == true ? RedirectToAction("Brands") : RedirectToAction("BrandDetails", new { id = brandId });
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View("CreateCategory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(string name, string? description, IFormFile? logo, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(name), "Name is required.");
                return View("CreateCategory");
            }

            string? logoFileName = null;
            string? logoDataUrl = null;

            if (logo is not null && logo.Length > 0)
            {
                logoFileName = logo.FileName;
                using var ms = new MemoryStream();
                await logo.CopyToAsync(ms, ct);
                var base64 = Convert.ToBase64String(ms.ToArray());
                var mime = logo.ContentType; 
                logoDataUrl = $"data:{mime};base64,{base64}";
            }

            var payload = new
            {
                Name = name,
                Description = description,
                LogoFileName = logoFileName,
                LogoFileContent = logoDataUrl
            };

            var apiUrl = $"{_baseUrl}/manage/create/category";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, payload, ct);

            if (!apiResponse.IsSuccessStatusCode)
            {
                var msg = $"API error: {(int)apiResponse.StatusCode}";
                ModelState.AddModelError(string.Empty, msg);
                return View("CreateCategory");
            }

            var resp = await apiResponse.Content.ReadFromJsonAsync<CreateCategoryForManageResponse>();
            if (resp is null || resp.Success == false)
            {
                ModelState.AddModelError(string.Empty, resp?.Message ?? "Failed to create category.");
                return View("CreateCategory");
            }

            TempData["SuccessMessage"] = resp.Message ?? "Category created.";
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            if (id <= 0) return NotFound();

            var apiUrl = $"{_baseUrl}/manage/category/{id}";
            var apiResponse = await _httpClient.GetAsync(apiUrl);
            if (!apiResponse.IsSuccessStatusCode)
                return View("NotFoundCategory");

            var category = await apiResponse.Content.ReadFromJsonAsync<GetCategoryForManageResponse>();
            if (category is null || category.Success == false)
                return View("NotFoundCategory", category);

            return View("EditCategory", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, string name, string? description, IFormFile? logo, bool? removeLogo, CancellationToken ct)
        {
            if (id <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid category id.");
                return RedirectToAction(nameof(Categories));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(name), "Name is required.");
                return await EditCategory(id);
            }

            string? logoFileName = null;
            string? logoDataUrl = null;

            if (logo is not null && logo.Length > 0)
            {
                logoFileName = logo.FileName;
                using var ms = new MemoryStream();
                await logo.CopyToAsync(ms, ct);
                var base64 = Convert.ToBase64String(ms.ToArray());
                var mime = logo.ContentType;
                logoDataUrl = $"data:{mime};base64,{base64}";
            }

            var payload = new EditCategoryForManageCommand
            {
                Id = id,
                Name = name,
                Description = description,
                LogoFileName = logoFileName,
                LogoFileContent = logoDataUrl,
                RemoveLogo = removeLogo ?? false
            };

            var apiUrl = $"{_baseUrl}/manage/edit/category";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, payload, ct);

            if (!apiResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"API error: {(int)apiResponse.StatusCode}");
                return await EditCategory(id);
            }

            var resp = await apiResponse.Content.ReadFromJsonAsync<EditCategoryForManageResponse>();
            if (resp is null || resp.Success == false)
            {
                ModelState.AddModelError(string.Empty, resp?.Message ?? "Failed to edit category.");
                return await EditCategory(id);
            }

            TempData["SuccessMessage"] = resp.Message ?? "Category updated.";
            return RedirectToAction(nameof(CategoryDetails), new { id });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategoryLogo(int id, CancellationToken ct)
        {
            var apiUrl = $"{_baseUrl}/files/image?categoryId={id}";
            var apiResponse = await _httpClient.DeleteAsync(apiUrl, ct);

            if (apiResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                return NoContent();

            var body = await apiResponse.Content.ReadAsStringAsync(ct);
            return StatusCode((int)apiResponse.StatusCode, string.IsNullOrWhiteSpace(body) ? null : body);
        }

        [HttpGet]
        public IActionResult CreateBrand()
        {
            return View("CreateBrand");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrand(string name, string? description, IFormFile? logo, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(name), "Name is required.");
                return View("CreateBrand");
            }

            string? logoFileName = null;
            string? logoDataUrl = null;
            if (logo is not null && logo.Length > 0)
            {
                logoFileName = logo.FileName;
                using var ms = new MemoryStream();
                await logo.CopyToAsync(ms, ct);
                var base64 = Convert.ToBase64String(ms.ToArray());
                var mime = logo.ContentType;
                logoDataUrl = $"data:{mime};base64,{base64}";
            }

            var payload = new CreateBrandForManageCommand
            {
                Name = name,
                Description = description,
                LogoFileName = logoFileName,
                LogoFileContent = logoDataUrl
            };

            var apiUrl = $"{_baseUrl}/manage/create/brand";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, payload, ct);

            if (!apiResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"API error: {(int)apiResponse.StatusCode}");
                return View("CreateBrand");
            }

            var resp = await apiResponse.Content.ReadFromJsonAsync<CreateBrandForManageResponse>();
            if (resp is null || resp.Success == false)
            {
                ModelState.AddModelError(string.Empty, resp?.Message ?? "Failed to create brand.");
                return View("CreateBrand");
            }

            TempData["SuccessMessage"] = resp.Message ?? "Brand created.";
            return RedirectToAction(nameof(Brands));
        }

        [HttpGet]
        public async Task<IActionResult> EditBrand(int id)
        {
            if (id <= 0) return NotFound();

            var apiUrl = $"{_baseUrl}/manage/brand/{id}";
            var apiResponse = await _httpClient.GetAsync(apiUrl);
            if (!apiResponse.IsSuccessStatusCode)
                return View("NotFoundBrand");

            var brand = await apiResponse.Content.ReadFromJsonAsync<GetBrandForManageResponse>();
            if (brand is null || brand.Success == false)
                return View("NotFoundBrand", brand);

            return View("EditBrand", brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBrand(int id, string name, string? description, IFormFile? logo, bool? removeLogo, CancellationToken ct)
        {
            if (id <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid brand id.");
                return RedirectToAction(nameof(Brands));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(name), "Name is required.");
                return await EditBrand(id);
            }

            string? logoFileName = null;
            string? logoDataUrl = null;
            if (logo is not null && logo.Length > 0)
            {
                logoFileName = logo.FileName;
                using var ms = new MemoryStream();
                await logo.CopyToAsync(ms, ct);
                var base64 = Convert.ToBase64String(ms.ToArray());
                var mime = logo.ContentType;
                logoDataUrl = $"data:{mime};base64,{base64}";
            }

            var payload = new EditBrandForManageCommand
            {
                Id = id,
                Name = name,
                Description = description,
                LogoFileName = logoFileName,
                LogoFileContent = logoDataUrl,
                RemoveLogo = removeLogo ?? false
            };

            var apiUrl = $"{_baseUrl}/manage/edit/brand";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, payload, ct);

            if (!apiResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"API error: {(int)apiResponse.StatusCode}");
                return await EditBrand(id);
            }

            var resp = await apiResponse.Content.ReadFromJsonAsync<EditBrandForManageResponse>();
            if (resp is null || resp.Success == false)
            {
                ModelState.AddModelError(string.Empty, resp?.Message ?? "Failed to edit brand.");
                return await EditBrand(id);
            }

            TempData["SuccessMessage"] = resp.Message ?? "Brand updated.";
            return RedirectToAction(nameof(BrandDetails), new { id });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBrandLogo(int id, CancellationToken ct)
        {
            var apiUrl = $"{_baseUrl}/files/image?brandId={id}";
            var apiResponse = await _httpClient.DeleteAsync(apiUrl, ct);

            if (apiResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                return NoContent();

            var body = await apiResponse.Content.ReadAsStringAsync(ct);
            return StatusCode((int)apiResponse.StatusCode, string.IsNullOrWhiteSpace(body) ? null : body);
        }

        [HttpGet]
        public async Task<IActionResult> ProducerCountries([FromQuery] ProducerCountriesFilterVm filter)
        {
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var request = new GetAllProducerCountriesForManageQuery
            {
                Pagination = new Pagination { PageNumber = pageNumber, PageSize = pageSize },
                Id = filter.Id,
                Name = string.IsNullOrWhiteSpace(filter.Name) ? null : filter.Name.Trim()
            };

            var apiUrl = $"{_baseUrl}/manage/producer-countries";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, request);

            GetAllProducerCountriesForManageResponse data;
            if (!apiResponse.IsSuccessStatusCode)
            {
                data = new GetAllProducerCountriesForManageResponse
                {
                    Success = false,
                    StatusCode = (int)apiResponse.StatusCode,
                    Message = $"API error: {(int)apiResponse.StatusCode}",
                    TotalCount = 0,
                    Items = new List<GetAllProducerCountriesForManageItemsResponse>()
                };
            }
            else
            {
                data = await apiResponse.Content.ReadFromJsonAsync<GetAllProducerCountriesForManageResponse>()
                       ?? new GetAllProducerCountriesForManageResponse
                       {
                           Success = false,
                           StatusCode = 500,
                           Message = "Invalid API response",
                           TotalCount = 0,
                           Items = new List<GetAllProducerCountriesForManageItemsResponse>()
                       };
            }

            filter.PageNumber = pageNumber;
            filter.PageSize = pageSize;

            var vm = new ProducerCountriesManageViewModel
            {
                Filter = filter,
                Response = data
            };

            return View("ProducerCountries", vm);
        }

        [HttpGet]
        public IActionResult CreateProducerCountry()
        {
            return View("CreateProducerCountry");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProducerCountry(string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(name), "Name is required.");
                return View("CreateProducerCountry");
            }

            var payload = new CreateProducerCountryForManageCommand
            {
                Name = name.Trim()
            };

            var apiUrl = $"{_baseUrl}/manage/create/producer-country";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, payload, ct);

            if (!apiResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"API error: {(int)apiResponse.StatusCode}");
                return View("CreateProducerCountry");
            }

            var resp = await apiResponse.Content.ReadFromJsonAsync<CreateProducerCountryForManageResponse>();
            if (resp is null || resp.Success == false)
            {
                ModelState.AddModelError(string.Empty, resp?.Message ?? "Failed to create producer country.");
                return View("CreateProducerCountry");
            }

            TempData["SuccessMessage"] = resp.Message ?? "Producer country created.";
            return RedirectToAction(nameof(ProducerCountries));
        }

        [HttpGet]
        public async Task<IActionResult> EditProducerCountry(int id)
        {
            if (id <= 0) return NotFound();

            var apiUrl = $"{_baseUrl}/manage/producer-country/{id}";
            var apiResponse = await _httpClient.GetAsync(apiUrl);
            if (!apiResponse.IsSuccessStatusCode)
                return View("NotFound");

            var country = await apiResponse.Content.ReadFromJsonAsync<GetProducerCountryForManageResponse>();
            if (country is null || country.Success == false)
                return View("NotFound", country);

            return View("EditProducerCountry", country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProducerCountry(int id, string name, CancellationToken ct)
        {
            if (id <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid country id.");
                return RedirectToAction(nameof(ProducerCountries));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError(nameof(name), "Name is required.");
                return await EditProducerCountry(id);
            }

            var payload = new EditProducerCountryForManageCommand
            {
                Id = id,
                Name = name.Trim()
            };

            var apiUrl = $"{_baseUrl}/manage/edit/producer-country";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, payload, ct);

            if (!apiResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, $"API error: {(int)apiResponse.StatusCode}");
                return await EditProducerCountry(id);
            }

            var resp = await apiResponse.Content.ReadFromJsonAsync<EditProducerCountryForManageResponse>();
            if (resp is null || resp.Success == false)
            {
                ModelState.AddModelError(string.Empty, resp?.Message ?? "Failed to edit producer country.");
                return await EditProducerCountry(id);
            }

            TempData["SuccessMessage"] = resp.Message ?? "Producer country updated.";
            return RedirectToAction(nameof(ProducerCountries));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProducerCountry(int id, bool? intoGrid, CancellationToken ct)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "არასწორი ქვეყნის ID.";
                return RedirectToAction(nameof(ProducerCountries));
            }

            var apiUrl = $"{_baseUrl}/manage/delete/producer-country/{id}";

            var apiResponse = await _httpClient.DeleteAsync(apiUrl, ct);

            if (!apiResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = $"ქვეყნის წაშლა ვერ მოხერხდა. API {(int)apiResponse.StatusCode}";
                return RedirectToAction(nameof(ProducerCountries));
            }

            var resp = await apiResponse.Content
                .ReadFromJsonAsync<DeleteProducerCountryForManageResponse>(cancellationToken: ct);

            TempData[(resp?.Success ?? false) ? "SuccessMessage" : "ErrorMessage"]
                = resp?.Message ?? "ქვეყნის წაშლა ვერ მოხერხდა.";

            return RedirectToAction(nameof(ProducerCountries));
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var categories = await LoadCategories();
            var brands = await LoadBrands();
            var countries = await LoadCountries();

            ViewBag.Categories = categories;
            ViewBag.Brands = brands;
            ViewBag.Countries = countries;

            ViewBag.ProductTypes = Enum.GetValues(typeof(Likano.Domain.Enums.ProductType))
            .Cast<Likano.Domain.Enums.ProductType>()
            .Select(pt => new
            {
                Value = (int)pt,
                Name = pt.ToString()
            }).ToList();

            return View("CreateProduct");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(string title, string? description, decimal? price, int categoryId,
        int? brandId, int? producerCountryId, string? material, decimal? length, decimal? width,
        decimal? height, string? color, List<IFormFile>? photos, int? mainPhotoIndex, string? code, string? seoTitle, ProductType? productType, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError(nameof(title), "სათაური სავალდებულოა.");
                await PopulateProductDropdowns();
                return View("CreateProduct");
            }
            if (photos != null && photos.Count > 10)
            {
                ModelState.AddModelError("photos", "მაქსიმუმ 10 ფოტო შეგიძლიათ ატვირთოთ.");
                await PopulateProductDropdowns();
                return View("CreateProduct");
            }

            var photoPayloads = new List<object>();
            if (photos != null && photos.Count > 0)
            {
                for (int i = 0; i < photos.Count; i++)
                {
                    var photo = photos[i];
                    if (photo.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await photo.CopyToAsync(ms, ct);
                        var base64 = Convert.ToBase64String(ms.ToArray());
                        var mime = photo.ContentType;
                        photoPayloads.Add(new
                        {
                            FileName = photo.FileName,
                            FileContent = $"data:{mime};base64,{base64}",
                            IsMain = (mainPhotoIndex.HasValue && mainPhotoIndex.Value == i) || (!mainPhotoIndex.HasValue && i == 0)
                        });
                    }
                }
            }

            var payload = new
            {
                Title = title,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                BrandId = brandId,
                ProducerCountryId = producerCountryId,
                Material = material,
                Length = length,
                Width = width,
                Height = height,
                Color = color,
                Images = photoPayloads,
                Code = code,
                SeoTitle = seoTitle,
                ProductType = productType
            };

            var apiUrl = $"{_baseUrl}/manage/product/create";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, payload, ct);

            if (!apiResponse.IsSuccessStatusCode)
            {
                var msg = $"API error: {(int)apiResponse.StatusCode}";
                ModelState.AddModelError(string.Empty, msg);
                await PopulateProductDropdowns();
                return View("CreateProduct");
            }

            var resp = await apiResponse.Content.ReadFromJsonAsync<CreateProductForManageResponse>();
            if (resp is null || resp.Success == false)
            {
                ModelState.AddModelError(string.Empty, resp?.Message ?? "მოხდა შეცდომა");
                await PopulateProductDropdowns();
                return View("CreateProduct");
            }

            TempData["SuccessMessage"] = resp.Message ?? "პროდუქტი წარმატებით შეიქმნა";
            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            if (id <= 0)
                return NotFound();

            var apiUrl = $"{_baseUrl}/manage/product/{id}";
            var apiResponse = await _httpClient.GetAsync(apiUrl);
            if (!apiResponse.IsSuccessStatusCode)
                return View("NotFound");

            var product = await apiResponse.Content.ReadFromJsonAsync<GetProductForManageResponse>();
            if (product == null || product.Success == false)
                return View("NotFound");

            var imagesUrl = $"{_baseUrl}/manage/product/{id}/images";
            var imagesResponse = await _httpClient.GetAsync(imagesUrl);
            var existingImages = new List<ProductImageDto>();
            if (imagesResponse.IsSuccessStatusCode)
            {
                existingImages = await imagesResponse.Content.ReadFromJsonAsync<List<ProductImageDto>>() ?? new();
            }

            var categories = await LoadCategories();
            var brands = await LoadBrands();
            var countries = await LoadCountries();

            ViewBag.ProductTypes = Enum.GetValues(typeof(Likano.Domain.Enums.ProductType))
            .Cast<Likano.Domain.Enums.ProductType>()
            .Select(pt => new
            {
                Value = (int)pt,
                Name = pt.ToString()
            }).ToList();

            var vm = new EditProductViewModel
            {
                Product = product,
                Categories = categories,
                Brands = brands,
                Countries = countries,
                ExistingImages = existingImages
            };

            return View("EditProduct", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, string title,  string? description, decimal? price, int categoryId, int? brandId, int? producerCountryId, 
            string? material, decimal? length, decimal? width, decimal? height, string? color, List<IFormFile>? photos, int mainPhotoIndex, int? existingMainImageId, string? deletedImageIds, string? code, string? seoTitle, ProductType? productType, CancellationToken ct)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "არასწორი პროდუქტის ID.";
                return RedirectToAction(nameof(Products));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError(nameof(title), "სათაური აუცილებელია.");
                return await EditProduct(id);
            }

            var newImages = new List<PhotoUploadDto>();
            if (photos != null && photos.Count > 0)
            {
                for (int i = 0; i < photos.Count; i++)
                {
                    var photo = photos[i];
                    if (photo.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await photo.CopyToAsync(ms, ct);
                        var base64 = Convert.ToBase64String(ms.ToArray());
                        var mime = photo.ContentType;
                        var dataUrl = $"data:{mime};base64,{base64}";

                        newImages.Add(new PhotoUploadDto
                        {
                            FileName = photo.FileName,
                            FileContent = dataUrl,
                            IsMain = i == mainPhotoIndex && !existingMainImageId.HasValue
                        });
                    }
                }
            }

            var deletedIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(deletedImageIds))
            {
                deletedIds = deletedImageIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.TryParse(x, out var val) ? val : 0)
                    .Where(x => x > 0)
                    .ToList();
            }

            var payload = new
            {
                Id = id,
                Title = title,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                BrandId = brandId,
                ProducerCountryId = producerCountryId,
                Material = material,
                Length = length,
                Width = width,
                Height = height,
                Color = color,
                NewImages = newImages,
                MainImageId = existingMainImageId,
                DeletedImageIds = deletedIds,
                Code = code,
                SeoTitle = seoTitle,
                ProductType = productType
            };

            var apiUrl = $"{_baseUrl}/manage/edit/product";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, payload, ct);

            if (!apiResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "პროდუქტის რედაქტირება ვერ მოხერხდა.";
                return await EditProduct(id);
            }

            var resp = await apiResponse.Content.ReadFromJsonAsync<EditProductForManageResponse>();
            TempData[(resp?.Success ?? false) ? "SuccessMessage" : "ErrorMessage"] = resp?.Message ?? "შეცდომა.";

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProductType(int productId, ProductType newType)
        {
            var apiUrl = $"{_baseUrl}/manage/change-product-type";
            var command = new ChangeTypeCommand
            {
                ProductId = productId,
                NewType = newType
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, command);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "ტიპის ცვლილება ვერ მოხერხდა";
                return RedirectToAction("Details", new { id = productId });
            }

            var result = await response.Content.ReadFromJsonAsync<ChangeTypeResponse>();
            TempData[(bool)result!.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

            return RedirectToAction("Details", new { id = productId });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProductAvailableStatus(int productId, bool isAvailable, bool? intoGrid)
        {
            var command = new ChangeAvailableStatusCommand
            {
                ProductId = productId,
                IsAvailable = isAvailable
            };

            var apiUrl = $"{_baseUrl}/manage/product/available-status";
            var apiResponse = await _httpClient.PostAsJsonAsync(apiUrl, command);

            if (apiResponse.IsSuccessStatusCode)
            {
                return intoGrid == true ? RedirectToAction("Products") : RedirectToAction("Details", new { id = productId });
            }

            return RedirectToAction("Products");
        }

        private async Task PopulateProductDropdowns()
        {
            ViewBag.Categories = await LoadCategories();
            ViewBag.Brands = await LoadBrands();
            ViewBag.Countries = await LoadCountries();
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
