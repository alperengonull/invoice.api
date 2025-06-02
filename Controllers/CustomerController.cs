using Microsoft.AspNetCore.Mvc;
using InvoiceApi.Models;
using InvoiceApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InvoiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCustomer([FromBody] Customer newCustomer)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (string.IsNullOrEmpty(newCustomer.TaxNumber) || string.IsNullOrEmpty(newCustomer.Title))
                return BadRequest(new { message = "TaxNumber ve Title zorunludur." });

            if (await _context.Customer.AnyAsync(c => c.TaxNumber == newCustomer.TaxNumber))
                return Conflict(new { message = "Bu TaxNumber zaten kullanımda." });

            newCustomer.UserId = userId;
            newCustomer.RecordDate = DateTime.UtcNow;

            await _context.Customer.AddAsync(newCustomer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Müşteri eklendi",
                customerId = newCustomer.CustomerId
            });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetCustomers()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var customers = await _context.Customer
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var customer = await _context.Customer
                .FirstOrDefaultAsync(c => c.CustomerId == id && c.UserId == userId);

            if (customer == null)
                return NotFound(new { message = "Müşteri bulunamadı" });

            return Ok(customer);
        }
    }
}