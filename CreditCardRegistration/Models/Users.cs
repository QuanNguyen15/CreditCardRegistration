using System.ComponentModel.DataAnnotations;

namespace CreditCardRegistration.Models
{
    // Represents a user in the system
    public class User
    {
        [Key]
        public int UserID { get; set; } // Primary key

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } // User's full name

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } // User's email address

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } // User's phone number (e.g., +84123456789)

        [Required]
        public DateTime DateOfBirth { get; set; } // User's date of birth

        [Required]
        [StringLength(20)]
        public string IDNumber { get; set; } // User's ID number (CMND/CCCD/Passport)

        [Required]
        public decimal MonthlyIncome { get; set; } // User's monthly income (VNĐ)

        [StringLength(50)]
        public string Occupation { get; set; } // User's occupation (optional)

        [StringLength(200)]
        public string Address { get; set; } // User's address (optional)

        [Required]
        [StringLength(50)]
        public string Username { get; set; } // Username for login

        [Required]
        [StringLength(200)]
        public string PasswordHash { get; set; } // Password (hashed for security)
    }
}