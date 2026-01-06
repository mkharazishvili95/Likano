using Likano.Application.Common.Models;
using Likano.Application.DTOs;
using Likano.Application.Features.Category.Queries.GetAll;
using Likano.Application.Features.Manage.Product.Queries.Get;
using Likano.Application.Features.Manage.Product.Queries.GetAll;
using Likano.Web.Models.Manage;
using Microsoft.AspNetCore.Mvc;

namespace Likano.Web.Controllers
{
    public class ManageController : Controller
    {
        readonly HttpClient _httpClient;
        readonly string _baseUrl;

        public ManageController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseUrl = configuration["ApiSettings:BaseUrl"]!;
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

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var apiUrl = $"{_baseUrl}/manage/product/{id}";
            var apiResponse = await _httpClient.GetAsync(apiUrl);

            if (!apiResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)apiResponse.StatusCode, $"API error: {(int)apiResponse.StatusCode}");
            }

            var product = await apiResponse.Content.ReadFromJsonAsync<GetProductForManageResponse>();
            if (product == null || product.Success == false)
            {
                return NotFound(product?.Message ?? "Product not found");
            }

            return View("Details", product);
        }
    }
}
