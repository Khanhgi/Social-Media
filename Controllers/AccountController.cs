using Microsoft.AspNetCore.Mvc;
using Social_Media.Models;
using System.Text;
using Newtonsoft.Json;
using Neo4j.Driver;
using System.Net;

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
            try
            {
                var apiUrl = "https://localhost:7112/api/APIAccount/Register";

                var json = JsonConvert.SerializeObject(newUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                response.EnsureSuccessStatusCode();

                return RedirectToAction("Login", "Account");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", "Unable to connect to the server. Please try again later.");

                return BadRequest("Invalid password format. Please follow the password requirements.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Registration failed. Please try again.");
                return View("Login", new AuthenticationViewModel());
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
            try
            {
                var apiUrl = "https://localhost:7112/api/APIAccount/Login";

                var json = JsonConvert.SerializeObject(loginModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                response.EnsureSuccessStatusCode();

                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewData["ErrorMessage"] = "*Invalid Login Information. Please try again";
                }
                else
                {
                    ViewData["ErrorConnectServer"] = "Unable to connect to the server. Please try again later";
                }

                return View("Login", new AuthenticationViewModel());
            }
            catch (Exception)
            {
                // Handle other types of exceptions
                ViewData["ErrorMessage"] = "*Invalid Login Information. Please try again";
                return View("Login", new AuthenticationViewModel());
            }
        }

    }
}
