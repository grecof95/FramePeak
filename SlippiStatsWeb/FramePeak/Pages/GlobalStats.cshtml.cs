using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FramePeak.Data;
using Microsoft.Extensions.Configuration;
using static Dapper.SqlMapper;

namespace FramePeak.Pages
{
    public class GlobalModel : PageModel
    {
        private readonly GlobalData _repo;

        [BindProperty(SupportsGet = true)]
        public int UserId { get; set; }
        public string Username { get; set; }
        public int GlobalMatchCount { get; set; }
        public string GlobalAvgDamageDealt { get; set; }
        public string GlobalAvgDamageTaken { get; set; }
        public List<string> GlobalTop3CharactersWinRate { get; set; } = new();
        public List<string> GlobalTop3CharactersUsage { get; set; } = new();
        public List<string> GlobalBot3CharactersWinRate { get; set; } = new();
        public string GlobalLCancelPercent { get; set; }

        public GlobalModel(IConfiguration configuration)
        {
            var settings = new Settings();
            configuration.Bind(settings);
            _repo = new GlobalData(settings.ConnString);
        }

        public void OnGet()
        {
            UserId = HttpContext.Session.GetInt32("UserID") ?? -1;
            Username = HttpContext.Session.GetString("Username");

            if (UserId <= 0 || string.IsNullOrEmpty(Username))
            {
                Response.Redirect("/Index");
                return;
            }
            GlobalMatchCount = _repo.GetGlobalMatchCount();
            GlobalAvgDamageDealt = _repo.GetGlobalDamageDealt();
            GlobalAvgDamageTaken = _repo.GetGlobalDamageTaken();
            GlobalTop3CharactersWinRate = _repo.GetGlobalTop3CharacterWinRate();
            GlobalTop3CharactersUsage = _repo.GetGlobalTop3CharacterUsage();
            GlobalBot3CharactersWinRate = _repo.GetGlobalBot3CharacterWinRate();
            GlobalLCancelPercent = _repo.GetGlobalOverallLCancelPercent();
        }

        static Settings BindSettings()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) //ask about which of these are optional
                .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                .Build();
            var settings = new Settings();
            configuration.Bind(settings);
            return settings;
        }
    }
}
