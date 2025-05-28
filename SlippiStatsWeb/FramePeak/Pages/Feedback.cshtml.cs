using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FramePeak.Pages
{
    public class FeedbackModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Topic { get; set; }

        [BindProperty]
        public string Feedback { get; set; }

        public IActionResult OnPost()
        {
            return RedirectToPage("/FeedbackConfirmation");
        }
    }
}
