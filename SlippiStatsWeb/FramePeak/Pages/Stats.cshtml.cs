using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FramePeak.Data;
using Microsoft.Extensions.Configuration;
using static Dapper.SqlMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FramePeak.Data;
using Microsoft.Extensions.Configuration;

namespace FramePeak.Pages
{
    public class StatsModel : PageModel
    {
        private readonly HomeData _repo;

        [BindProperty(SupportsGet = true)]
        public int UserId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Character { get; set; }

        public List<string> CharacterList { get; set; } = new();
        public string Username { get; set; }
        public string CharWinCount { get; set; }
        public string CharLossCount { get; set; }
        public string CharAvgStocksTaken { get; set; }
        public string CharAvgStocksLost { get; set; }
        public string CharAvgDmgDealt { get; set; }
        public string CharAvgDmgTaken { get; set; }
        public string CharAvgLCancel { get; set; }


        public StatsModel(IConfiguration configuration)
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

            CharacterList = _repo.GetCharactersPlayedByUser(UserId);

            // Now Character (from the GET) will be used for your filtered queries
            if (!string.IsNullOrWhiteSpace(Character))
            {
                CharWinCount = _repo.GetCharacterWins(UserId, Character);
                CharLossCount = _repo.GetCharacterLosses(UserId, Character);
                CharAvgStocksTaken = _repo.GetCharacterAvgStocksTaken(UserId, Character);
                CharAvgStocksLost = _repo.GetCharacterAvgStocksLost(UserId, Character);
                CharAvgDmgDealt = _repo.GetCharacterAvgDamageDealt(UserId, Character);
                CharAvgDmgTaken = _repo.GetCharacterAvgDamageTaken(UserId, Character);
                CharAvgLCancel = _repo.GetCharacterAvgLCancel(UserId, Character);
            }

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
