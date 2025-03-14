using CreditCardRegistration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CreditCardRegistration.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string UserFullName { get; set; }
        public string CardNumber { get; set; }
        public IList<CardType> CardTypes { get; set; }

        public async Task OnGetAsync()
        {
            // Load card types for display
            CardTypes = await _context.CardTypes.ToListAsync();

            // Check for welcome message after registration
            UserFullName = HttpContext.Session.GetString("UserFullName");
            CardNumber = HttpContext.Session.GetString("CardNumber");

            if (UserFullName != null)
            {
                HttpContext.Session.Remove("UserFullName");
                HttpContext.Session.Remove("CardNumber");
            }

            // Xóa CardTypeID khỏi session khi tải trang Index
            HttpContext.Session.Remove("CardTypeID");
        }

        public IActionResult OnPostSelectCardType(int cardTypeId)
        {
            // Store selected CardTypeID in session
            HttpContext.Session.SetInt32("CardTypeID", cardTypeId);
            return RedirectToPage("/PersonalDetails");
        }
    }
}