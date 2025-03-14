using CreditCardRegistration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CreditCardRegistration.Pages
{
    public class CardDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CardDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public User User { get; set; }
        public IList<CreditCard> CreditCards { get; set; }
        public string CardRegistrationTime { get; set; } // New property to pass to the view

        public IActionResult OnGet()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("LoggedInUserID");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            // Load user
            User = _context.Users.Find(userId.Value);
            if (User == null)
            {
                return RedirectToPage("/Login");
            }

            // Load credit cards for the user, including related CardType
            CreditCards = _context.CreditCards
                .Include(c => c.CardType)
                .Where(c => c.UserID == userId.Value)
                .ToList();

            // Update card status if 1 minute has passed
            foreach (var card in CreditCards)
            {
                if (card.Status == "Pending" && (DateTime.Now - card.CreatedDate).TotalMinutes >= 1)
                {
                    card.Status = "Active";
                }
            }

            // Get CardRegistrationTime from session and pass to the view
            CardRegistrationTime = HttpContext.Session.GetString("CardRegistrationTime");

            _context.SaveChanges();

            return Page();
        }

        public IActionResult OnPostUpdateCardStatus()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("LoggedInUserID");
            if (!userId.HasValue)
            {
                return new JsonResult(new { updated = false });
            }

            // Load credit cards for the user
            var creditCards = _context.CreditCards
                .Where(c => c.UserID == userId.Value)
                .ToList();

            bool updated = false;
            foreach (var card in creditCards)
            {
                if (card.Status == "Pending" && (DateTime.Now - card.CreatedDate).TotalMinutes >= 1)
                {
                    card.Status = "Active";
                    updated = true;
                }
            }

            _context.SaveChanges();

            return new JsonResult(new { updated = updated });
        }

        public IActionResult OnPostClearRegistrationTime()
        {
            HttpContext.Session.Remove("CardRegistrationTime");
            return new OkResult();
        }

        // Mask a card number for display on the card template (e.g., 1234-56**-****-****)
        public string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length != 19)
                return "N/A";

            // Display only the first 6 digits, mask the rest
            return $"{cardNumber.Substring(0, 6)}**-****-****";
        }
    }
}