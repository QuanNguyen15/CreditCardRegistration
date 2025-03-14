using CreditCardRegistration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;

namespace CreditCardRegistration.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50)]
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            // If already logged in, redirect to Index
            if (HttpContext.Session.GetInt32("LoggedInUserID") != null)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Find user by username
            var user = _context.Users.FirstOrDefault(u => u.Username == Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
            {
                ErrorMessage = "Username or password is wrong.";
                return Page();
            }

            // Login successful, store UserID and Username in session
            HttpContext.Session.SetInt32("LoggedInUserID", user.UserID);
            HttpContext.Session.SetString("LoggedInUsername", user.Username);

            return RedirectToPage("/Index");
        }

        public IActionResult OnPostLogout()
        {
            // Logout: clear session
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}