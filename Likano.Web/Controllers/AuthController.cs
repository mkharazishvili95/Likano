using Likano.Web.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Likano.Web.Controllers
{
    public class AuthController : Controller
    {
        readonly HttpClient _httpClient;
        readonly string _baseUrl;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseUrl = configuration["ApiSettings:BaseUrl"]!;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var apiUrl = $"{_baseUrl.TrimEnd('/')}/auth/register";

                var response = await _httpClient.PostAsJsonAsync(apiUrl, new
                {
                    userName = model.UserName,
                    password = model.Password,
                    confirmPassword = model.ConfirmPassword
                });

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "თქვენ წარმატებით დარეგისტრირდით.";
                    return RedirectToAction(nameof(Login));
                }

                var errorText = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Registration failed: {errorText}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}