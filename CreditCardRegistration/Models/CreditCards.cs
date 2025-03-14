using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCardRegistration.Models
{
    // Represents a credit card registered by a user
    public class CreditCard
    {
        [Key]
        public int CreditCardID { get; set; } // Primary key

        [Required]
        public int UserID { get; set; } // Foreign key to User

        [Required]
        public int CardTypeID { get; set; } // Foreign key to CardType

        [Required]
        [StringLength(19)]
        public string CardNumber { get; set; } // Card number (e.g., 1234-5678-9012-3456)

        [Required]
        [StringLength(5)]
        public string ExpiryDate { get; set; } // Expiry date (e.g., 12-25)

        [Required]
        [StringLength(3)]
        public string CVV { get; set; } // CVV code (e.g., 123)

        [Required]
        [ForeignKey("UserID")]
        public User User { get; set; } // Navigation property to User

        [Required]
        [ForeignKey("CardTypeID")]
        public CardType CardType { get; set; } // Navigation property to CardType

        [StringLength(200)]
        public string FrontID { get; set; } // Path to uploaded front
        [StringLength(200)]
        public string BackID { get; set; }  // Path to upload back

        [StringLength(200)]
        public string DocumentPath { get; set; } // Path to uploaded identification document

        [StringLength(20)]
        public string Status { get; set; } // Status: Pending, Approved

        public DateTime CreatedDate { get; set; } // Date the card registration was created
    }
}