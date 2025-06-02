using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace InvoiceApi.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string TaxNumber { get; set; } = null!;

        [Required]
        public string Title { get; set; } = null!;

        public string? Address { get; set; }

        public string? Email { get; set; }

        public DateTime RecordDate { get; set; }

        // Foreign key
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }  // nullable yaptık

        public List<Invoice>? Invoice { get; set; }  // nullable yaptık
    }
}
