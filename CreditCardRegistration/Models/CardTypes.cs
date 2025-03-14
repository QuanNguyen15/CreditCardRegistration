using System.ComponentModel.DataAnnotations;

namespace CreditCardRegistration.Models
{
    // Represents a type of credit card (e.g., Visa, Mastercard)
    public class CardType
    {
        [Key]
        public int CardTypeID { get; set; } // Primary key

        [Required]
        [StringLength(50)]
        public string CardTypeName { get; set; } // Name of the card type (e.g., Visa, Mastercard)

        [StringLength(200)]
        public string Benefits { get; set; } // Benefits of the card type (optional description)
    }
}