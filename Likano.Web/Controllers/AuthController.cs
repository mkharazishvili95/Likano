using Likano.Application.Features.Auth.Commands.Login;
using Likano.Web.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Likano.Web.Controllers
{
    public class AuthController : Controller
    {
        readonly HttpClient _httpClient;
        readonly string _baseUrl;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("API");
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
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var apiUrl = $"{_baseUrl.TrimEnd('/')}/auth/login";

                var response = await _httpClient.PostAsJsonAsync(apiUrl, new
                {
                    userName = model.UserName,
                    password = model.Password
                });

                if (response.IsSuccessStatusCode)
                {
                    var payload = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
                    if (payload is not null && !string.IsNullOrWhiteSpace(payload.AccessToken))
                    {
                        var accessOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(2)
                        };
                        Response.Cookies.Append("access_token", payload.AccessToken, accessOptions);

                        if (!string.IsNullOrWhiteSpace(payload.RefreshToken))
                        {
                            var refreshOptions = new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                Expires = DateTimeOffset.UtcNow.AddDays(14)
                            };
                            Response.Cookies.Append("refresh_token", payload.RefreshToken, refreshOptions);
                        }

                        string? role = null;
                        try
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var jwt = handler.ReadJwtToken(payload.AccessToken);
                            role = jwt.Claims.FirstOrDefault(c =>
                                       c.Type == ClaimTypes.Role ||
                                       c.Type == "role" ||
                                       c.Type == "roles")?.Value;
                        }
                        catch
                        {
                        }

                        TempData["SuccessMessage"] = "წარმატებით შეხვდით სისტემაში.";
                        if (string.Equals(role, nameof(Likano.Domain.Enums.User.UserType.Admin), StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Main", "Manage");
                        }
                        return RedirectToAction("Index", "Home");
                    }

                    TempData["ErrorMessage"] = "არასწორი მომხმარებელი ან პაროლი";
                    return View(model);
                }

                var errorText = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Login failed: {errorText}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"შეცდომა: {ex.Message}");
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(bool revokeAll = false)
        {
            var baseUrl = _baseUrl?.TrimEnd('/') ?? "";
            var apiUrl = $"{baseUrl}/api/Auth/logout";

            try
            {
                var token = Request.Cookies["access_token"];
                using var req = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                {
                    Content = JsonContent.Create(new { revokeAll })
                };
                if (!string.IsNullOrWhiteSpace(token))
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var resp = await _httpClient.SendAsync(req);

                Response.Cookies.Delete("access_token");
                Response.Cookies.Delete("refresh_token");

                TempData["SuccessMessage"] = resp.IsSuccessStatusCode
                    ? "გამოსვლა შესრულებულია."
                    : "მოხდა შეცდომა.";
            }
            catch
            {
                Response.Cookies.Delete("access_token");
                Response.Cookies.Delete("refresh_token");
                TempData["ErrorMessage"] = "Logout ვერ შესრულდა სრულად, ლოკალური გამოსვლა დასრულებულია.";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}