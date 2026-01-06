using Likano.Application.Common.Models;
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
            if (!apiResponse.IsSuccessStatusCode)
            {
                var vmError = new ProductsManageViewModel
                {
                    Filter = filter,
                    Response = new GetAllProductsForManageResponse
                    {
                        Success = false,
                        StatusCode = (int)apiResponse.StatusCode,
                        Message = $"API error: {(int)apiResponse.StatusCode}",
                        TotalCount = 0,
                        Items = new List<GetAllProductsForManageItemsResponse>()
                    }
                };
                return View("Products", vmError);
            }

            var data = await apiResponse.Content.ReadFromJsonAsync<GetAllProductsForManageResponse>()
                       ?? new GetAllProductsForManageResponse
                       {
                           Success = false,
                           StatusCode = 500,
                           Message = "Invalid API response",
                           TotalCount = 0
                       };

            filter.PageNumber = pageNumber;
            filter.PageSize = pageSize;

            var vm = new ProductsManageViewModel
            {
                Filter = filter,
                Response = data
            };

            return View("Products", vm);
        }
    }
}
