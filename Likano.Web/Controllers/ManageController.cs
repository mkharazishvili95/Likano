using Likano.Application.Common.Models;
using Likano.Application.DTOs;
using Likano.Application.Features.Category.Queries.GetAll;
using Likano.Application.Features.Manage.Brand.Commands.Change;
using Likano.Application.Features.Manage.Brand.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Brand.Commands.Create;
using Likano.Application.Features.Manage.Brand.Queries.Get;
using Likano.Application.Features.Manage.Brand.Queries.GetAll;
using Likano.Application.Features.Manage.Category.Commands.ChangeStatus;
using Likano.Application.Features.Manage.Category.Commands.Create;
using Likano.Application.Features.Manage.Category.Commands.Edit;
using Likano.Application.Features.Manage.Category.Queries.Get;
using Likano.Application.Features.Manage.Category.Queries.GetAll;
using Likano.Application.Features.Manage.Product.Commands.ChangeCategory;
using Likano.Application.Features.Manage.Product.Commands.ChangeStatus;
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
                UpdateDateTo = filter.UpdateDateTo
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

            var categoriesUrl = $"{_baseUrl}/category/all";
            var categoriesRequest = new GetAllCategoriesQuery
            {
                Pagination = new Pagination { PageNumber = 1, PageSize = 1000 },
                SearchString = null
            };
            var categoriesResponse = await _httpClient.PostAsJsonAsync(categoriesUrl, categoriesRequest);

            var categories = new List<CategoryDtoForManage>();
            if (categoriesResponse.IsSuccessStatusCode)
            {
                var categoriesData = await categoriesResponse.Content.ReadFromJsonAsync<GetAllCategoriesResponse>();
                if (categoriesData?.Items != null)
                {
                    categories = categoriesData.Items
                        .Select(c => new CategoryDtoForManage { Id = c.Id, Name = c.Name })
                        .ToList();
                }
            }

            filter.PageNumber = pageNumber;
            filter.PageSize = pageSize;

            var vm = new ProductsManageViewModel
            {
                Filter = filter,
                Response = data,
                Categories = categories
            };

            return View("Products", vm);
        }
        //Products:
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var apiUrl = $"{_baseUrl}/manage/product/{id}";
            var apiResponse = await _httpClient.GetAsync(apiUrl);

            if (!apiResponse.IsSuccessStatusCode)
            {
                return View("NotFound", new GetProductForManageResponse
                {
                    Success = false,
                    Message = $"API error: {(int)apiResponse.StatusCode}"
                });
            }
            var product = await apiResponse.Content.ReadFromJsonAsync<GetProductForManageResponse>();
            if (product == null || product.Success == false)
            {
                return View("NotFound", product ?? new GetProductForManageResponse
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return View("Details", product);
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
        public IActionResult Forbidden()
        {
            return View();
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
    }
}
