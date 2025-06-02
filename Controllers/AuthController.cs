using Microsoft.AspNetCore.Mvc;
using InvoiceApi.Models;
using InvoiceApi.Data;
using Microsoft.AspNetCore.Identity;
using InvoiceApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace InvoiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<AppUser> _passwordHasher;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<AppUser>();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AppUser user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
                return BadRequest("Kullanıcı adı ve şifre gereklidir.");

            if (user.Password.Length < 6)
                return BadRequest("Şifre en az 6 karakter olmalıdır.");

            if (await _context.AppUser.AnyAsync(u => u.UserName == user.UserName))
                return BadRequest("Bu kullanıcı adı zaten alınmış.");

            user.Password = _passwordHasher.HashPassword(user, user.Password);
            user.RecordDate = DateTime.UtcNow;

            _context.AppUser.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Kayıt başarılı.",
                userId = user.UserId
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Kullanıcı adı ve şifre zorunludur.");

            var user = await _context.AppUser.FirstOrDefaultAsync(u => u.UserName == model.UserName);
            if (user == null)
                return Unauthorized("Geçersiz kullanıcı adı.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (result != PasswordVerificationResult.Success)
                return Unauthorized("Geçersiz şifre.");

            var tokenResult = JwtHelper.GenerateToken(user, _configuration);

            return Ok(new
            {
                message = "Giriş başarılı",
                userId = user.UserId,
                userName = user.UserName,
                token = tokenResult.Token,
                expiresIn = tokenResult.ExpiresAt
            });
        }

        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            // Token geçerliyse buraya ulaşılabilir
            return Ok(new { message = "Token geçerli" });
        }
    }

    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}