using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Likano.Web.Controllers
{
    public class RobotsController : Controller
    {
        [HttpGet("robots.txt")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public IActionResult Index()
        {
            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Allow: /");
            sb.AppendLine("Disallow: /Manage/");
            sb.AppendLine("Disallow: /Error/");
            sb.AppendLine("Disallow: /Auth/");      
            sb.AppendLine("Disallow: /Shared/");     
            sb.AppendLine();
            sb.AppendLine("Sitemap: https://www.likano.ge/sitemap.xml");

            return Content(sb.ToString(), "text/plain", Encoding.UTF8);
        }
    }
}