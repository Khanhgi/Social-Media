using Microsoft.AspNetCore.Mvc;
using Social_Media.Models;
using System.Text;
using Newtonsoft.Json;
using Neo4j.Driver;

namespace Social_Media.Controllers
{
    public class AccountController : Controller
    {
        private readonly IDriver _driver;
        private readonly HttpClient _httpClient;

        public AccountController(IDriver driver)
        {
            _driver = driver;
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User newUser)
        {
            var apiUrl = "https://localhost:7112/api/APIAccount/Register";

            var json = JsonConvert.SerializeObject(newUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "Registration failed. Please try again.");
                return View(newUser);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login loginModel)
        {
            var apiUrl = "https://localhost:7112/api/APIAccount/Login";

            var json = JsonConvert.SerializeObject(loginModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt. Please try again.");
                return View(loginModel);
            }
        }
    }
}
