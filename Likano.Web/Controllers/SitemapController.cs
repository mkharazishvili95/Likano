using Likano.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;

namespace Likano.Web.Controllers
{
    public class SitemapController : Controller
    {
        readonly HttpClient _httpClient;
        readonly string _baseUrl;

        public SitemapController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _baseUrl = configuration["ApiSettings:BaseUrl"]!;
        }

        [HttpGet("sitemap.xml")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            var baseUrl = "https://www.likano.ge";
            var sitemapItems = new List<SitemapItem>
            {
                new SitemapItem { Url = baseUrl, Priority = 1.0, ChangeFrequency = "daily" },
                new SitemapItem { Url = $"{baseUrl}/contact", Priority = 0.8, ChangeFrequency = "monthly" }
            };

            try
            {
                var productsRequest = new
                {
                    Pagination = new { PageNumber = 1, PageSize = 10000 },
                    SearchString = (string?)null,
                    IsAvailable = true
                };

                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Product/search", productsRequest);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    if (result?.items != null)
                    {
                        foreach (var product in result.items)
                        {
                            if (product?.id != null && product?.seoTitle != null)
                            {
                                var productUrl = $"{baseUrl}/Details/{product.seoTitle}-{product.id}";
                                sitemapItems.Add(new SitemapItem
                                {
                                    Url = productUrl,
                                    Priority = 0.9,
                                    ChangeFrequency = "weekly",
                                    LastModified = product?.updateDate ?? DateTime.UtcNow
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            var xml = GenerateSitemapXml(sitemapItems);
            return Content(xml, "application/xml", Encoding.UTF8);
        }

        private string GenerateSitemapXml(List<SitemapItem> items)
        {
            var ns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
            var sitemap = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement(ns + "urlset",
                    items.Select(item =>
                        new XElement(ns + "url",
                            new XElement(ns + "loc", item.Url),
                            new XElement(ns + "lastmod", item.LastModified.ToString("yyyy-MM-dd")),
                            new XElement(ns + "changefreq", item.ChangeFrequency),
                            new XElement(ns + "priority", item.Priority.ToString("F1"))
                        )
                    )
                )
            );

            return sitemap.ToString();
        }
    }
}