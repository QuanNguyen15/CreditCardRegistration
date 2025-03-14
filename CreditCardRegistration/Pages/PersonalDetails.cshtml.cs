using CreditCardRegistration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using System.IO;

namespace CreditCardRegistration.Pages
{
    public class PersonalDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PersonalDetailsModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50)]
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Full Name cannot be longer than 100 characters.")]
        public string FullName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phone Prefix is required.")]
        [StringLength(5, ErrorMessage = "Phone Prefix cannot be longer than 5 characters.")]
        public string PhonePrefix { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phone Number is required.")]
        [StringLength(15, MinimumLength = 9, ErrorMessage = "Phone Number must be between 9 and 15 digits.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone Number must contain only digits.")]
        public string PhoneNumber { get; set; }

        public string Phone => $"{PhonePrefix}{PhoneNumber}";

        [BindProperty]
        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "ID Number is required.")]
        [StringLength(20, ErrorMessage = "ID Number cannot be longer than 20 characters.")]
        public string IDNumber { get; set; }

        [BindProperty]
        public IFormFile FrontIDPhoto { get; set; }

        [BindProperty]
        public IFormFile BackIDPhoto { get; set; }

        [BindProperty]
        public IFormFile DocumentUpload { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Monthly Income is required.")]
        public decimal MonthlyIncome { get; set; }

        [BindProperty]
        [StringLength(50, ErrorMessage = "Occupation cannot be longer than 50 characters.")]
        public string Occupation { get; set; }

        [BindProperty]
        [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters.")]
        public string Address { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "You must agree to the terms and conditions.")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the terms and conditions.")]
        public bool TermsAgreed { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("CardTypeID") == null)
            {
                return RedirectToPage("/Index");
            }

            // Khôi phục dữ liệu từ session
            RestoreFormData();
            return Page();
        }

        // Xử lý Step 1 (Account Information)
        public IActionResult OnPostStep1()
        {
            // Loại bỏ các trường không liên quan đến Step 1 khỏi ModelState
            ModelState.Remove("FullName");
            ModelState.Remove("Email");
            ModelState.Remove("PhonePrefix");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("DateOfBirth");
            ModelState.Remove("IDNumber");
            ModelState.Remove("FrontIDPhoto");
            ModelState.Remove("BackIDPhoto");
            ModelState.Remove("DocumentUpload");
            ModelState.Remove("MonthlyIncome");
            ModelState.Remove("Occupation");
            ModelState.Remove("Address");
            ModelState.Remove("TermsAgreed");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new JsonResult(new { success = false, errors = errors });
            }

            if (_context.Users.Any(u => u.Username == Username))
            {
                ModelState.AddModelError("Username", "Username already exists.");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new JsonResult(new { success = false, errors = errors });
            }

            // Lưu tạm dữ liệu vào session
            HttpContext.Session.SetString("Temp_Username", Username);
            HttpContext.Session.SetString("Temp_Password", Password);
            HttpContext.Session.SetString("Temp_ConfirmPassword", ConfirmPassword);

            return new JsonResult(new { success = true, nextStep = 2 });
        }

        // Xử lý Step 2 (Personal Information)
        public IActionResult OnPostStep2()
        {
            // Loại bỏ các trường không liên quan đến Step 2 khỏi ModelState
            ModelState.Remove("Username");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("FrontIDPhoto");
            ModelState.Remove("BackIDPhoto");
            ModelState.Remove("DocumentUpload");
            ModelState.Remove("MonthlyIncome");
            ModelState.Remove("Occupation");
            ModelState.Remove("Address");
            ModelState.Remove("TermsAgreed");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new JsonResult(new { success = false, errors = errors });
            }

            if (_context.Users.Any(u => u.Email == Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new JsonResult(new { success = false, errors = errors });
            }

            // Lưu tạm dữ liệu vào session
            HttpContext.Session.SetString("Temp_FullName", FullName);
            HttpContext.Session.SetString("Temp_Email", Email);
            HttpContext.Session.SetString("Temp_PhonePrefix", PhonePrefix);
            HttpContext.Session.SetString("Temp_PhoneNumber", PhoneNumber);
            HttpContext.Session.SetString("Temp_DateOfBirth", DateOfBirth.ToString("o"));
            HttpContext.Session.SetString("Temp_IDNumber", IDNumber);

            return new JsonResult(new { success = true, nextStep = 3 });
        }

        // Xử lý Step 3 (Upload Verification Documents)
        public IActionResult OnPostStep3()
        {
            // Loại bỏ các trường không liên quan đến Step 3 khỏi ModelState
            ModelState.Remove("Username");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("FullName");
            ModelState.Remove("Email");
            ModelState.Remove("PhonePrefix");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("DateOfBirth");
            ModelState.Remove("IDNumber");
            ModelState.Remove("MonthlyIncome");
            ModelState.Remove("Occupation");
            ModelState.Remove("Address");
            ModelState.Remove("TermsAgreed");

            if (FrontIDPhoto == null || BackIDPhoto == null || DocumentUpload == null)
            {
                ModelState.AddModelError("", "All files in Step 3 are required.");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new JsonResult(new { success = false, errors = errors });
            }

            string frontPhotoPath = null;
            string backPhotoPath = null;
            string documentPath = null;

            try
            {
                var tempUploadsFolder = Path.Combine(_environment.WebRootPath, "temp_uploads");
                if (!Directory.Exists(tempUploadsFolder))
                {
                    Directory.CreateDirectory(tempUploadsFolder);
                }

                // Upload FrontIDPhoto
                if (FrontIDPhoto != null && IsValidFile(FrontIDPhoto, new[] { ".jpg", ".jpeg", ".png", ".gif" }))
                {
                    var frontFileName = Guid.NewGuid().ToString() + Path.GetExtension(FrontIDPhoto.FileName);
                    var frontFilePath = Path.Combine(tempUploadsFolder, frontFileName);
                    using (var fileStream = new FileStream(frontFilePath, FileMode.Create))
                    {
                        FrontIDPhoto.CopyTo(fileStream);
                    }
                    frontPhotoPath = $"/temp_uploads/{frontFileName}";
                }
                else
                {
                    ModelState.AddModelError("FrontIDPhoto", "Front ID Photo must be an image (jpg, png, gif) and less than 5MB.");
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return new JsonResult(new { success = false, errors = errors });
                }

                // Upload BackIDPhoto
                if (BackIDPhoto != null && IsValidFile(BackIDPhoto, new[] { ".jpg", ".jpeg", ".png", ".gif" }))
                {
                    var backFileName = Guid.NewGuid().ToString() + Path.GetExtension(BackIDPhoto.FileName);
                    var backFilePath = Path.Combine(tempUploadsFolder, backFileName);
                    using (var fileStream = new FileStream(backFilePath, FileMode.Create))
                    {
                        BackIDPhoto.CopyTo(fileStream);
                    }
                    backPhotoPath = $"/temp_uploads/{backFileName}";
                }
                else
                {
                    ModelState.AddModelError("BackIDPhoto", "Back ID Photo must be an image (jpg, png, gif) and less than 5MB.");
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return new JsonResult(new { success = false, errors = errors });
                }

                // Upload DocumentUpload
                if (DocumentUpload != null && IsValidFile(DocumentUpload, new[] { ".pdf", ".jpg", ".jpeg", ".png" }))
                {
                    var docFileName = Guid.NewGuid().ToString() + Path.GetExtension(DocumentUpload.FileName);
                    var docFilePath = Path.Combine(tempUploadsFolder, docFileName);
                    using (var fileStream = new FileStream(docFilePath, FileMode.Create))
                    {
                        DocumentUpload.CopyTo(fileStream);
                    }
                    documentPath = $"/temp_uploads/{docFileName}";
                }
                else
                {
                    ModelState.AddModelError("DocumentUpload", "Additional Identification Document must be a PDF, JPG, or PNG file and less than 5MB.");
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return new JsonResult(new { success = false, errors = errors });
                }

                // Lưu tạm đường dẫn file vào session
                HttpContext.Session.SetString("Temp_FrontIDPhoto", frontPhotoPath);
                HttpContext.Session.SetString("Temp_BackIDPhoto", backPhotoPath);
                HttpContext.Session.SetString("Temp_DocumentUpload", documentPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error uploading files: {ex.Message}");
                ErrorMessage = "An error occurred while uploading files. Please try again.";
                return new JsonResult(new { success = false, errors = new List<string> { ErrorMessage } });
            }

            return new JsonResult(new { success = true, nextStep = 4 });
        }

        // Xử lý Step 4 và Submit (Additional Information)
        public IActionResult OnPost()
        {
            // Khôi phục dữ liệu từ session trước khi validate
            RestoreFormData();

            // Log giá trị TermsAgreed trước khi validate
            System.Diagnostics.Debug.WriteLine($"Before Validation - TermsAgreed: {TermsAgreed}");

            // Bind dữ liệu từ form trước khi validate
            if (Request.HasFormContentType)
            {
                var formData = Request.Form;
                if (bool.TryParse(formData["TermsAgreed"], out bool termsAgreedFromForm))
                {
                    TermsAgreed = termsAgreedFromForm;
                    System.Diagnostics.Debug.WriteLine($"TermsAgreed from Form: {TermsAgreed}");
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"OnPost ModelState Error: {error.ErrorMessage}");
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return new JsonResult(new { success = false, errors = errors });
            }

            // Lưu TermsAgreed vào session
            HttpContext.Session.SetString("Temp_TermsAgreed", TermsAgreed.ToString());
            HttpContext.Session.SetString("Temp_MonthlyIncome", MonthlyIncome.ToString());
            HttpContext.Session.SetString("Temp_Occupation", Occupation ?? string.Empty);
            HttpContext.Session.SetString("Temp_Address", Address ?? string.Empty);

            // Lấy dữ liệu từ session
            Username = HttpContext.Session.GetString("Temp_Username") ?? Username;
            Password = HttpContext.Session.GetString("Temp_Password") ?? Password;
            FullName = HttpContext.Session.GetString("Temp_FullName") ?? FullName;
            Email = HttpContext.Session.GetString("Temp_Email") ?? Email;
            PhonePrefix = HttpContext.Session.GetString("Temp_PhonePrefix") ?? PhonePrefix;
            PhoneNumber = HttpContext.Session.GetString("Temp_PhoneNumber") ?? PhoneNumber;
            if (HttpContext.Session.GetString("Temp_DateOfBirth") != null)
            {
                DateOfBirth = DateTime.Parse(HttpContext.Session.GetString("Temp_DateOfBirth"));
            }
            IDNumber = HttpContext.Session.GetString("Temp_IDNumber") ?? IDNumber;
            string frontPhotoPath = HttpContext.Session.GetString("Temp_FrontIDPhoto");
            string backPhotoPath = HttpContext.Session.GetString("Temp_BackIDPhoto");
            string documentPath = HttpContext.Session.GetString("Temp_DocumentUpload");

            // Di chuyển file từ temp_uploads sang uploads
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (!string.IsNullOrEmpty(frontPhotoPath))
            {
                var newFrontPath = MoveFileFromTempToUploads(frontPhotoPath, uploadsFolder);
                frontPhotoPath = newFrontPath;
            }

            if (!string.IsNullOrEmpty(backPhotoPath))
            {
                var newBackPath = MoveFileFromTempToUploads(backPhotoPath, uploadsFolder);
                backPhotoPath = newBackPath;
            }

            if (!string.IsNullOrEmpty(documentPath))
            {
                var newDocPath = MoveFileFromTempToUploads(documentPath, uploadsFolder);
                documentPath = newDocPath;
            }

            // Checking login process
            var userId = HttpContext.Session.GetInt32("LoggedInUserID");
            User user;

            if (userId.HasValue)
            {
                user = _context.Users.Find(userId.Value);
                if (user == null)
                {
                    ErrorMessage = "User not found.";
                    return new JsonResult(new { success = false, errors = new List<string> { ErrorMessage } });
                }

                user.FullName = FullName;
                user.Email = Email;
                user.Phone = Phone;
                user.DateOfBirth = DateOfBirth;
                user.IDNumber = IDNumber;
                user.MonthlyIncome = MonthlyIncome;
                user.Occupation = Occupation;
                user.Address = Address;
            }
            else
            {
                user = new User
                {
                    Username = Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password),
                    FullName = FullName,
                    Email = Email,
                    Phone = Phone,
                    DateOfBirth = DateOfBirth,
                    IDNumber = IDNumber,
                    MonthlyIncome = MonthlyIncome,
                    Occupation = Occupation,
                    Address = Address
                };

                _context.Users.Add(user);
                _context.SaveChanges();
            }

            var cardTypeId = HttpContext.Session.GetInt32("CardTypeID");
            if (!cardTypeId.HasValue)
            {
                ErrorMessage = "Card Type is required.";
                return new JsonResult(new { success = false, errors = new List<string> { ErrorMessage } });
            }

            var creditCard = new CreditCard
            {
                UserID = user.UserID,
                CardTypeID = cardTypeId.Value,
                CardNumber = GenerateCardNumber(),
                ExpiryDate = GenerateExpiryDate(),
                CVV = GenerateCVV(),
                FrontID = frontPhotoPath,
                BackID = backPhotoPath,
                DocumentPath = documentPath,
                Status = "Pending",
                CreatedDate = DateTime.Now
            };

            _context.CreditCards.Add(creditCard);

            try
            {
                _context.SaveChanges();

                if (!userId.HasValue)
                {
                    HttpContext.Session.SetInt32("LoggedInUserID", user.UserID);
                    HttpContext.Session.SetString("LoggedInUsername", user.Username);
                }

                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetString("UserFullName", user.FullName);
                HttpContext.Session.SetString("CardNumber", creditCard.CardNumber);
                HttpContext.Session.SetString("CardRegistrationTime", DateTime.Now.ToString("o"));

                ClearTempData();
                return new JsonResult(new { success = true }); // Trả về JSON khi thành công
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving data: {ex.Message}");
                ErrorMessage = "An error occurred during registration. Please try again.";
                return new JsonResult(new { success = false, errors = new List<string> { ErrorMessage } });
            }
        }

        // Khôi phục dữ liệu từ session khi người dùng quay lại
        private void RestoreFormData()
        {
            Username = HttpContext.Session.GetString("Temp_Username") ?? Username;
            Password = HttpContext.Session.GetString("Temp_Password") ?? Password;
            ConfirmPassword = HttpContext.Session.GetString("Temp_ConfirmPassword") ?? ConfirmPassword;
            FullName = HttpContext.Session.GetString("Temp_FullName") ?? FullName;
            Email = HttpContext.Session.GetString("Temp_Email") ?? Email;
            PhonePrefix = HttpContext.Session.GetString("Temp_PhonePrefix") ?? PhonePrefix;
            PhoneNumber = HttpContext.Session.GetString("Temp_PhoneNumber") ?? PhoneNumber;
            if (HttpContext.Session.GetString("Temp_DateOfBirth") != null)
            {
                DateOfBirth = DateTime.Parse(HttpContext.Session.GetString("Temp_DateOfBirth"));
            }
            IDNumber = HttpContext.Session.GetString("Temp_IDNumber") ?? IDNumber;
            MonthlyIncome = HttpContext.Session.GetString("Temp_MonthlyIncome") != null ? decimal.Parse(HttpContext.Session.GetString("Temp_MonthlyIncome")) : MonthlyIncome;
            Occupation = HttpContext.Session.GetString("Temp_Occupation") ?? Occupation;
            Address = HttpContext.Session.GetString("Temp_Address") ?? Address;
            TermsAgreed = HttpContext.Session.GetString("Temp_TermsAgreed") == "true" || TermsAgreed;
        }

        // Xóa dữ liệu tạm trong session
        private void ClearTempData()
        {
            HttpContext.Session.Remove("Temp_Username");
            HttpContext.Session.Remove("Temp_Password");
            HttpContext.Session.Remove("Temp_ConfirmPassword");
            HttpContext.Session.Remove("Temp_FullName");
            HttpContext.Session.Remove("Temp_Email");
            HttpContext.Session.Remove("Temp_PhonePrefix");
            HttpContext.Session.Remove("Temp_PhoneNumber");
            HttpContext.Session.Remove("Temp_DateOfBirth");
            HttpContext.Session.Remove("Temp_IDNumber");
            HttpContext.Session.Remove("Temp_FrontIDPhoto");
            HttpContext.Session.Remove("Temp_BackIDPhoto");
            HttpContext.Session.Remove("Temp_DocumentUpload");
            HttpContext.Session.Remove("Temp_MonthlyIncome");
            HttpContext.Session.Remove("Temp_Occupation");
            HttpContext.Session.Remove("Temp_Address");
            HttpContext.Session.Remove("Temp_TermsAgreed");
        }

        // Di chuyển file từ temp_uploads sang uploads
        private string MoveFileFromTempToUploads(string tempPath, string uploadsFolder)
        {
            if (string.IsNullOrEmpty(tempPath))
                return null;

            var fileName = Path.GetFileName(tempPath);
            var newPath = Path.Combine(uploadsFolder, fileName);
            var tempFullPath = Path.Combine(_environment.WebRootPath, tempPath.TrimStart('/'));

            if (System.IO.File.Exists(tempFullPath))
            {
                System.IO.File.Move(tempFullPath, newPath);
                return $"/uploads/{fileName}";
            }

            return null;
        }

        // Method to validate file uploads
        private bool IsValidFile(IFormFile file, string[] allowedExtensions)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file size (limit to 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return false;

            // Check file format
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return false;

            return true;
        }

        // Generate a random card number (format: 1234-5678-9012-3456) with Luhn Algorithm
        private string GenerateCardNumber()
        {
            var random = new Random();
            int[] cardNumber = new int[16];

            // Đặt prefix (ví dụ: 4xxx cho Visa)
            cardNumber[0] = 4;

            // Điền 14 chữ số ngẫu nhiên (chừa chữ số cuối để tính Luhn)
            for (int i = 1; i < 15; i++)
            {
                cardNumber[i] = random.Next(0, 10);
            }

            // Tính chữ số kiểm tra (Luhn Algorithm)
            int sum = 0;
            for (int i = 0; i < 15; i++)
            {
                int digit = cardNumber[i];
                if (i % 2 == 0) // Nhân đôi các vị trí lẻ (tính từ 0)
                {
                    digit *= 2;
                    if (digit > 9) digit -= 9;
                }
                sum += digit;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            cardNumber[15] = checkDigit;

            // Format card number
            return $"{cardNumber[0]}{cardNumber[1]}{cardNumber[2]}{cardNumber[3]}-{cardNumber[4]}{cardNumber[5]}{cardNumber[6]}{cardNumber[7]}-{cardNumber[8]}{cardNumber[9]}{cardNumber[10]}{cardNumber[11]}-{cardNumber[12]}{cardNumber[13]}{cardNumber[14]}{cardNumber[15]}";
        }

        // Generate a random expiry date (format: MM-YY)
        private string GenerateExpiryDate()
        {
            var random = new Random();
            var month = random.Next(1, 13).ToString("D2");
            var year = (DateTime.Now.Year % 100 + random.Next(1, 6)).ToString("D2");
            return $"{month}-{year}";
        }

        // Generate a random CVV (3 digits)
        private string GenerateCVV()
        {
            var random = new Random();
            return random.Next(100, 999).ToString();
        }
    }
}