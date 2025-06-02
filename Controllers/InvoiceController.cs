using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoiceApi.Models;
using InvoiceApi.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InvoiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoiceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveInvoice([FromBody] Invoice input)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (input == null || input.CustomerId <= 0)
                return BadRequest(new { message = "Geçerli müşteri ID'si gereklidir." });

            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.CustomerId == input.CustomerId && c.UserId == userId);

            if (customer == null)
                return BadRequest(new { message = "Geçerli müşteri bulunamadı." });

            input.UserId = userId;
            input.RecordDate = DateTime.UtcNow;

            await _context.Invoice.AddAsync(input);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Fatura başarıyla kaydedildi.",
                invoiceId = input.InvoiceId
            });
        }

        [HttpGet("list/{customerId}")]
        public async Task<IActionResult> GetInvoicesByCustomer(int customerId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var invoices = await _context.Invoice
                .Include(i => i.Customer)
                .Where(i => i.UserId == userId && i.CustomerId == customerId)
                .Select(i => new
                {
                    i.InvoiceId,
                    i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate.ToString("yyyy-MM-dd"),
                    i.TotalAmount,
                    Customer = new { i.Customer.CustomerId, i.Customer.Title }
                })
                .ToListAsync();

            return Ok(invoices);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateInvoice([FromBody] Invoice updatedInvoice)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var existingInvoice = await _context.Invoice
                .FirstOrDefaultAsync(i => i.InvoiceId == updatedInvoice.InvoiceId && i.UserId == userId);

            if (existingInvoice == null)
                return NotFound(new { message = "Fatura bulunamadı." });

            existingInvoice.InvoiceNumber = updatedInvoice.InvoiceNumber;
            existingInvoice.InvoiceDate = updatedInvoice.InvoiceDate;
            existingInvoice.TotalAmount = updatedInvoice.TotalAmount;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Fatura güncellendi." });
        }

        [HttpDelete("delete/{invoiceId}")]
        public async Task<IActionResult> DeleteInvoice(int invoiceId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var invoice = await _context.Invoice
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.UserId == userId);

            if (invoice == null)
                return NotFound(new { message = "Fatura bulunamadı." });

            _context.Invoice.Remove(invoice);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Fatura silindi." });
        }


        [HttpPut("update-total/{invoiceId}")]
        public async Task<IActionResult> UpdateInvoiceTotal(int invoiceId, [FromBody] UpdateTotalRequest request)
        {
            var invoice = await _context.Invoice.FindAsync(invoiceId);
            if (invoice == null) return NotFound();

            invoice.TotalAmount = request.NewTotalAmount;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{invoiceId}")]
        public async Task<IActionResult> GetInvoice(int invoiceId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var invoice = await _context.Invoice
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.UserId == userId);

            if (invoice == null)
                return NotFound(new { message = "Fatura bulunamadı." });

            return Ok(new
            {
                invoice.InvoiceId,
                invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
                invoice.TotalAmount,
                Customer = new { invoice.Customer.CustomerId, invoice.Customer.Title }
            });
        }

        public class UpdateTotalRequest
        {
            public decimal NewTotalAmount { get; set; }
        }
    }
}