using Likano.Infrastructure.Queries.Product.Models;
using Likano.Web.Models;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
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
