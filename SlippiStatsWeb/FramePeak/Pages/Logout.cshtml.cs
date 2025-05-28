using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FramePeak.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // To add: Clear any session or temp data if needed
            return RedirectToPage("/Index"); // Redirect to Login page
        }
    }
}
