using Likano.Application.Features.Brand.Queries.GetAll;
using Likano.Application.Features.Category.Queries.GetAll;
using Likano.Application.Features.ProducerCountry.Queries.GetAll;
using Likano.Infrastructure.Queries.Product.Models;
using Likano.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Likano.Web.Controllers
{
    public class HomeController : Controller
    {
        readonly HttpClient _httpClient;
        readonly string _baseUrl;
        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _baseUrl = configuration["ApiSettings:BaseUrl"]!;
        }

        [HttpPost]
        public async Task<IActionResult> SearchProducts([FromBody] GetAllProductsForSearchQuery query)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Product/search", query);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GetAllProductsForSearchResponse>();
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categoriesUrl = $"{_baseUrl}/category/all";
            var categoriesRequest = new { Pagination = new { PageNumber = 1, PageSize = 1000 }, SearchString = (string?)null };
            var categoriesResponse = await _httpClient.PostAsJsonAsync(categoriesUrl, categoriesRequest);

            var categories = new List<CategoryDtoForSearch>();
            if (categoriesResponse.IsSuccessStatusCode)
            {
                var categoriesData = await categoriesResponse.Content.ReadFromJsonAsync<GetAllCategoriesResponse>();
                if (categoriesData?.Items != null)
                {
                    categories = categoriesData.Items
                        .Select(c => new CategoryDtoForSearch { CategoryId = c.Id, Name = c.Name })
                        .ToList();
                }
            }

            var brandsUrl = $"{_baseUrl}/brand/all";
            var brandsRequest = new { Pagination = new { PageNumber = 1, PageSize = 1000 }, Name = (string?)null };
            var brandsResponse = await _httpClient.PostAsJsonAsync(brandsUrl, brandsRequest);

            var brands = new List<BrandDtoForSearch>();
            if (brandsResponse.IsSuccessStatusCode)
            {
                var brandsData = await brandsResponse.Content.ReadFromJsonAsync<GetAllBrandsResponse>();
                if (brandsData?.Items != null)
                {
                    brands = brandsData.Items
                        .Select(b => new BrandDtoForSearch { BrandId = b.Id, Name = b.Name })
                        .ToList();
                }
            }

            var countriesUrl = $"{_baseUrl}/producercountry/all";
            var countriesRequest = new { Pagination = new { PageNumber = 1, PageSize = 1000 }, Name = (string?)null };
            var countriesResponse = await _httpClient.PostAsJsonAsync(countriesUrl, countriesRequest);

            var countries = new List<ProducerCountryDtoForSearch>();
            if (countriesResponse.IsSuccessStatusCode)
            {
                var countriesData = await countriesResponse.Content.ReadFromJsonAsync<GetAllProducerCountriesResponse>();
                if (countriesData?.Items != null)
                {
                    countries = countriesData.Items
                        .Select(c => new ProducerCountryDtoForSearch { ProducerCountryId = c.Id, Name = c.Name })
                        .ToList();
                }
            }

            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            ViewBag.Brands = new SelectList(brands, "BrandId", "Name");
            ViewBag.ProducerCountries = new SelectList(countries, "ProducerCountryId", "Name");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}