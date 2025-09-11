using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stajApi.Data;
using System.Linq;
using System.Threading.Tasks;

namespace stajApi.Controllers
{
    /// <summary>
    /// Yonetici ile ilgili işlemleri içerir.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Kullanıcıyı username’e göre getir
        [HttpGet("getbyusername")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _context.Users
                .Where(u => u.Username == username)
                .Select(u => new
                {
                    id = u.Id,
                    username = u.Username
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            return Ok(user);
        }

        // 🔹 Tüm kullanıcıları getir
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    id = u.Id,
                    username = u.Username
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
