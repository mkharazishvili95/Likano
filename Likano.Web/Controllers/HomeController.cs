using Likano.Application.Features.Brand.Queries.GetAll;
using Likano.Application.Features.Category.Queries.GetAll;
using Likano.Application.Features.ProducerCountry.Queries.GetAll;
using Likano.Infrastructure.Queries.Product.Models;
using Likano.Infrastructure.Queries.Product.Models.Details;
using Likano.Infrastructure.Queries.Product.Models.Similar;
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
        //not found default page:
        [Route("Shared/NotFound")]
        public IActionResult NotFoundPage()
        {
            return View("NotFound");
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
        public async Task<IActionResult> Index(int? categoryId)
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

            ViewBag.SelectedCategoryId = categoryId;

            return View();
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> ProductDetails(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Product/details/{id}");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GetProductDetailsResponse>();
            if (result == null || result.Id == null)
                return View("NotFound");

            var similarsResponse = await _httpClient.GetAsync($"{_baseUrl}/Product/similars?productId={id}");
            var similars = new List<SimilarProductDto>();
            if (similarsResponse.IsSuccessStatusCode)
            {
                var similarsResult = await similarsResponse.Content.ReadFromJsonAsync<GetSimilarProductsResponse>();
                if (similarsResult?.Items != null)
                    similars = similarsResult.Items
                        .Select(x => new SimilarProductDto
                        {
                            Id = (int)x.Id,
                            Title = (string?)x.Title,
                            ImageUrl = (string?)x.MainImage,
                            Price = (decimal?)x.Price
                        })
                        .ToList();
            }

            ViewBag.SimilarProducts = similars;

            return View("Details", result);
        }

        [HttpGet("similars/{id}")]
        public async Task<IActionResult> SimilarProducts(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Product/similars?productId={id}");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GetSimilarProductsResponse>();
            if (result == null || result.Items == null)
                return View("NotFound");

            return View("Similars", result.Items);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [Route("contact")]
        public IActionResult Contact()
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