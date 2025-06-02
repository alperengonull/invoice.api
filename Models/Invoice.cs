using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApi.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

        [Required]
        public required string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime RecordDate { get; set; }

        // Foreign keys
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]

        public Customer? Customer { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }
    }
}
