using Microsoft.AspNetCore.Mvc;
using InvoiceApi.Models;
using Microsoft.EntityFrameworkCore;
using InvoiceApi.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InvoiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Tüm endpoint'leri koruma altına al
    public class InvoiceLineController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoiceLineController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddLine([FromBody] InvoiceLine line)
        {
            if (line == null || line.InvoiceId <= 0)
                return BadRequest(new { message = "Geçersiz veri." });

            // Kullanıcıyı token'dan al
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            line.UserId = userId; // Kalemi oluşturan kullanıcıyı ayarla
            line.RecordDate = DateTime.UtcNow;

            await _context.InvoiceLine.AddAsync(line);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Kalem başarıyla eklendi.",
                lineId = line.InvoiceLineId
            });
        }

        [HttpGet("by-invoice/{invoiceId}")]
        public async Task<IActionResult> GetLinesByInvoice(int invoiceId)
        {
            var lines = await _context.InvoiceLine
                .Where(il => il.InvoiceId == invoiceId)
                .ToListAsync();

            return Ok(lines);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLine(int id)
        {
            // Kullanıcıyı token'dan al
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var line = await _context.InvoiceLine
                .FirstOrDefaultAsync(il => il.InvoiceLineId == id && il.UserId == userId);

            if (line == null)
                return NotFound(new { message = "Fatura kalemi bulunamadı veya silme yetkiniz yok." });

            _context.InvoiceLine.Remove(line);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Fatura kalemi başarıyla silindi." });
            
        }
    }
}