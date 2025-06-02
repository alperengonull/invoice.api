// using System.ComponentModel.DataAnnotations;

// namespace InvoiceApi.Models
// {
//     public class AppUser
//     {
//         [Key]
//         public int UserId { get; set; }

//         [Required]
//         public required string UserName { get; set; }

//         [Required]

//         public required string Password { get; set; }

//         public DateTime RecordDate { get; set; }

//     }
// }


using System.ComponentModel.DataAnnotations;

namespace InvoiceApi.Models
{
    public class AppUser
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public required string UserName { get; set; }

        [Required]
        public required string Password { get; set; }

        public DateTime RecordDate { get; set; }

        // Navigation properties
        public ICollection<Customer>? Customer { get; set; }
        public ICollection<Invoice>? Invoice { get; set; }
        public ICollection<InvoiceLine>? InvoiceLine { get; set; }
    }
}