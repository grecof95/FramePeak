using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FramePeak.Data;

namespace FramePeak.Pages.Home
{
    public class HomeModel : PageModel
    {
        private readonly FramePeakRepository _repo;

        public string Username { get; set; }
        public int MatchCount { get; set; }

        public HomeModel()
        {
            _repo = new FramePeakRepository(); // Initialize the repository
        }

        public void OnGet()
        {
            Username = TempData["Username"] as string;
            var userIdStr = TempData["UserID"] as string;

            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                MatchCount = _repo.GetMatchCountForUser(userId);
                TempData.Keep(); // keep username and userId
            }
            else
            {
                Response.Redirect("/Index"); // Redirect if no valid session
            }
        }
    }
}
