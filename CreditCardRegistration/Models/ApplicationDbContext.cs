using Microsoft.EntityFrameworkCore;

namespace CreditCardRegistration.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
    }
}