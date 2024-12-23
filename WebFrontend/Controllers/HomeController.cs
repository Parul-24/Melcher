using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using WebFrontend.Models;

namespace WebFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Prepare API request payload
            var loginPayload = new
            {
                Username = username,
                Password = password
            };

            // Convert payload to JSON
            var jsonPayload = JsonSerializer.Serialize(loginPayload);

            using (var httpClient = new HttpClient())
            {
                // Set up the API URL
                string apiUrl = "https://yourapiendpoint.com/api/login";

                // Configure HTTP request
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    // Send POST request to the API
                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Deserialize the API response
                        var responseData = JsonSerializer.Deserialize<dynamic>(await response.Content.ReadAsStringAsync());

                        // Extract token or success message from response
                        var token = responseData?.token;
                        var success = responseData?.success;

                        if (success)
                        {
                            // Save token in session or other storage (e.g., cookies)
                            HttpContext.Session.SetString("AuthToken", token.ToString());

                            // Redirect to a protected area of your app
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.Error = "Invalid credentials.";
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Login failed. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions
                    ViewBag.Error = $"An error occurred: {ex.Message}";
                }
            }

            // If login fails, return the view with an error message
            return View();
        }
    }
}