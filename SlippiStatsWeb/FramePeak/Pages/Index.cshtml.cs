using FramePeak.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FramePeak.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HomeData _repo;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;

            var settings = new Settings();
            configuration.Bind(settings);

            _repo = new HomeData(settings.ConnString);
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; } = "";

        public string SuccessMessage { get; set; } = "";

        public IActionResult OnPost(string action)
        {
            if (action == "login")
            {
                if (_repo.ValidateUser(Username, Password))
                {
                    int userId = _repo.GetUserId(Username, Password); 
                    HttpContext.Session.SetString("Username", Username);
                    HttpContext.Session.SetInt32("UserID", userId);
                    return RedirectToPage("/Home", new { userId });
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                    return Page();
                }
            }
            else if (action == "create")
            {
                var result = _repo.CreateUser(Username, Password);

                switch (result)
                {
                    case "empty":
                        ErrorMessage = "Please fill out both username and password fields.";
                        break;
                    case "exists":
                        ErrorMessage = "Username already exists. Please log in instead.";
                        break;
                    case "fail":
                        ErrorMessage = "Failed to create account. Please try again.";
                        break;
                    case "success":
                        SuccessMessage = "Account successfully created!";
                        break;
                }

                return Page();
            }

            ErrorMessage = "Unknown action.";
            return Page();
        }

        static Settings BindSettings()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) 
                .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                .Build();
            var settings = new Settings();
            configuration.Bind(settings);
            return settings;
        }
    }
}