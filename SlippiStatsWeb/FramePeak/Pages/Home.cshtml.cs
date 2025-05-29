using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FramePeak.Data;
using Microsoft.Extensions.Configuration;
using static Dapper.SqlMapper;

namespace FramePeak.Pages.Home
{
    public class HomeModel : PageModel
    {
        private readonly HomeData _repo;

        [BindProperty(SupportsGet = true)]
        public int UserId { get; set; }
        public string Username { get; set; }
        public int MatchCount { get; set; }
        public string MostPlayedCharacter { get; set; }
        public string MostPlayedOpponent { get; set; }
        public List<string> Top3CharactersWinRate { get; set; } = new();
        public List<string> Top3CharactersUsage { get; set; } = new();
        public List<string> Top3MapsWinRate { get; set; } = new();
        public string OverallLCancelPercent { get; set; }
        public string TopWinRateCharacter { get; set; }


        public HomeModel(IConfiguration configuration)
        {
            var settings = new Settings();
            configuration.Bind(settings);
            _repo = new HomeData(settings.ConnString);
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
            MatchCount = _repo.GetMatchCountForUser(UserId);
            MostPlayedCharacter = _repo.GetMostPlayedCharacter(UserId);
            MostPlayedOpponent = _repo.GetMostPlayedOpponent(UserId);
            Top3CharactersWinRate = _repo.GetUserTop3CharacterWinRate(UserId);
            Top3CharactersUsage = _repo.GetUserTop3CharacterUsage(UserId);
            Top3MapsWinRate = _repo.GetUserTop3Maps(UserId);
            OverallLCancelPercent = _repo.GetUserOverallLCancelPercent(UserId);
            TopWinRateCharacter = _repo.GetTopWinRateCharacter(UserId);

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
