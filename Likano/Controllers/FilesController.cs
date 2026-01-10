using Microsoft.AspNetCore.Mvc;

namespace Likano.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
